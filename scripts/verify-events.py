"""Run against a disposable local API process: python3 scripts/verify-events.py [base URL]."""
import concurrent.futures
import datetime
import json
import sys
import urllib.error
import urllib.parse
import urllib.request
import uuid

base = (sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5016").rstrip("/")
start = (datetime.datetime.now(datetime.timezone.utc) + datetime.timedelta(days=30)).isoformat()
payload = {"title": "  Workshop  ", "location": "  Sydney  ", "startTime": start, "capacity": 10}
created_ids = set()
checks = 0


def request(method, path, body=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(base + path, data=data, method=method,
                                 headers={"Content-Type": "application/json"})
    try:
        response = urllib.request.urlopen(req, timeout=10)
    except urllib.error.HTTPError as error:
        response = error
    with response:
        raw = response.read()
        return response.status, json.loads(raw) if raw else None, response.headers


def check(condition, message):
    global checks
    assert condition, message
    checks += 1


def create(body):
    status, event, headers = request("POST", "/events", body)
    assert status == 201, (status, event)
    return event, headers


try:
    status, initial, _ = request("GET", "/events")
    check(status == 200, "List must succeed")
    event, headers = create(payload)
    event_id = event["id"]
    created_ids.add(event_id)
    path = "/events/" + event_id
    check(event["title"] == "Workshop" and event["location"] == "Sydney", "Trim text")
    location_path = urllib.parse.urlsplit(headers["Location"]).path
    check(location_path.lower() == path, "Location identifies the new event")
    check(request("GET", location_path)[:2] == (200, event), "Following Location retrieves the created event")
    check(len(request("GET", "/events")[1]) == len(initial) + 1, "One shared store")

    invalid_fields = [
        {"title": None}, {"title": ""}, {"title": "  "},
        {"location": None}, {"location": ""}, {"location": "  "},
        {"capacity": 0}, {"capacity": -1},
        {"startTime": "2000-01-01T00:00:00Z"},
    ]
    for invalid in invalid_fields:
        for method, route in [("POST", "/events"), ("PUT", path)]:
            status, error, _ = request(method, route, payload | invalid)
            check(status == 400 and bool(error.get("error")), f"Reject {method} {invalid}")
        check(request("GET", path)[1] == event, "Rejected update must not change storage")
    check(len(request("GET", "/events")[1]) == len(initial) + 1, "Rejected creates add nothing")

    status, updated, _ = request("PUT", path, payload | {"title": "  Updated  ", "capacity": 12})
    check(status == 200 and updated["id"] == event_id and updated["title"] == "Updated"
          and updated["capacity"] == 12, "Update mapping and ID preservation")
    check(request("GET", path)[1] == updated, "Update visible to a later request")
    missing = "/events/" + str(uuid.uuid4())
    for method, body in [("GET", None), ("PUT", payload), ("PUT", payload | {"capacity": 0}),
                         ("DELETE", None)]:
        check(request(method, missing, body)[0] == 404, f"Missing resource: {method}")
    check(request("POST", "/events", {})[0] == 400, "Reject missing required values")

    # Use separate HTTP connections to exercise the singleton across overlapping requests.
    with concurrent.futures.ThreadPoolExecutor(max_workers=12) as pool:
        futures = [pool.submit(create, payload | {"title": f"Parallel {i}"}) for i in range(40)]
        batch = []
        for future in concurrent.futures.as_completed(futures):
            item, _ = future.result()
            created_ids.add(item["id"])
            batch.append(item)
        check(len({item["id"] for item in batch}) == 40, "Concurrent creates have unique IDs")
        listed = request("GET", "/events")[1]
        check(len(listed) == len(initial) + 41, "Concurrent creates are not lost")
        check(created_ids <= {item["id"] for item in listed}, "Every created event is listed")

        def update_and_read(i):
            body = payload | {"title": f"Version {i}", "location": f"Room {i}", "capacity": i + 1}
            status, result, _ = request("PUT", path, body)
            assert status == 200 and result["title"] == body["title"]
            assert result["location"] == body["location"] and result["capacity"] == i + 1
            status, snapshot, _ = request("GET", path)
            assert status == 200
            version = int(snapshot["title"].split()[-1])
            assert snapshot["location"] == f"Room {version}" and snapshot["capacity"] == version + 1
            assert request("GET", "/events")[0] == 200

        list(pool.map(update_and_read, range(60)))
        check(True, "Concurrent reads and writes return coherent event snapshots")
        statuses = list(pool.map(lambda _: request("DELETE", path)[0], range(12)))
        check(statuses.count(204) == 1 and statuses.count(404) == 11, "Only one deletion succeeds")
    check(request("GET", path)[0] == 404, "Deleted event is missing")
    print(f"PASS: {checks} HTTP checks, including concurrent creation, updates, reads and deletion")
finally:
    for event_id in created_ids:
        request("DELETE", "/events/" + event_id)
