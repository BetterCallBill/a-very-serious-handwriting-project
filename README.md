# Event Booking & Ticketing Platform

## Current progress

Last updated: 15 October 2026 (Australia/Sydney). See [status.md](status.md) for the current task and working agreement.

- **Current stage:** Week 1 fundamentals; session of 15 October, continuing service extraction.
- **Completed:** Every controller action now uses constructor-injected `EventService`. The existing singleton registration preserves one store across requests. Controller-owned storage and duplicate validation/mapping were removed. Service access uses a lock, and all returned events are copies so callers cannot mutate storage.
- **Verification:** API build passed with zero errors and the existing NU1903 warning for transitive `Microsoft.OpenApi` 2.0.0. The repeatable HTTP script passed 46 checks, including CRUD, following Location, invalid input, unchanged storage after rejection, missing resources, concurrent creates, coherent concurrent reads/updates, and exactly one successful concurrent deletion. A temporary console harness passed 14 direct service checks, including returned-object isolation and explicit validation time.
- **Working preference:** Follow the current AGENTS.md: complete requested implementation and verification; independent exercises only when explicitly requested or accepted. Older template-only notes are historical.
- **Understanding demonstrated:** Previously explained repeated deletion correctly. Today's code was assistant-implemented; DI, service lifetimes, locking, and snapshots remain unassessed. No new scores awarded.
- **Known limitations:** In-memory changes disappear on restart and are not shared between API processes. Locking serializes store access and does not detect stale client updates; the latest successful update wins. No authentication yet. The existing OpenAPI dependency warning remains ([advisory](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)); dependency remediation is a follow-up.
- **Next action:** Trace a POST through routing, model binding, controller activation, service validation, and response serialization. Explain singleton versus scoped lifetime before moving on to middleware and async/await within Week 1.
- **Optional understanding check:** If `AddSingleton<EventService>()` became `AddScoped<EventService>()`, would a GET in a later request find an event created by POST? Explain using `_events`.

## Run and verify locally

Start a disposable local API process:

```sh
dotnet run --project EventBooking.Api --launch-profile http
```

In a second terminal:

```sh
python3 scripts/verify-events.py http://localhost:5016
```

The script creates and cleans up its own events and sends concurrent requests. Run it against a local development instance with no other writers. It requires Python 3.9+ and no external Python packages. Build with `dotnet build EventBooking.Api`.

## Historical progress snapshot — 9 October 2026

Last updated: 9 October 2026 (Australia/Sydney).

Current stage: **Week 1 — C# and ASP.NET Core fundamentals, in progress.** This record reflects our chat and the saved implementation. Features completed with guidance are not evidence of independent mastery. No formal assessment scores have been assigned yet.

| Week | Focus and milestone | Status |
|---|---|---|
| 1 | C# and ASP.NET Core; in-memory Event Management API | In progress: list, lookup, and creation implemented with guidance |
| 2 | SQL, PostgreSQL, EF Core, reservations, and concurrency | Not started |
| 3 | React and TypeScript frontend | Not started |
| 4 | Authentication, authorisation, testing, and CI | Not started |
| 5 | System design, waitlists, background jobs, and recovery | Not started |
| 6 | Docker, AWS, deployment, and operations | Not started |
| 7 | Interview training and code quality | Not started |
| 8 | Final engineering assessment and portfolio | Not started |

**Week 1 work completed so far**

- Created and ran a controller-based ASP.NET Core Web API targeting .NET 10 in VS Code.
- Created an Event model with public properties: Id, Title, Location, StartTime, and Capacity.
- Implemented GET /events to return a collection of events. Confirmed that event values appear in JSON.
- Implemented GET /events/{id} using a GUID parameter, a lookup, and an explicit missing-event response. Source reviewed; successful and missing-ID HTTP results have not yet been shared.
- Replaced a per-request collection with shared in-memory storage, then changed an array to List<Event> to support adding events.
- Implemented POST /events with an explicit body parameter, a server-generated ID, and a CreatedAtAction response.
- Tested POST with curl and shared evidence of 201 Created, a generated ID, a Location header, and the submitted event data in the response body.
- Explained reference reassignment versus object mutation, predicted list counts correctly, and identified the Location header as the URL a client should follow.

**Remaining work and checks**

- Follow the POST response's Location URL and verify 200 with the same event and ID. This has been requested but not yet confirmed.
- Record results for a missing valid GUID, repeated GET requests with stable IDs, and an empty collection response.
- Add request validation and appropriate error responses.
- Implement event updates and deletion.
- Address safe concurrent access to shared in-memory storage before treating it as reliable API storage.
- Study and practise application responsibilities, interfaces, dependency injection, middleware, the full request lifecycle, exceptions, generics, LINQ, Task, and async/await.
- Write and test endpoints independently, then complete the Week 1 assessment before progressing to PostgreSQL.

**Current learning needs**

Collection creation, object initializer syntax, properties versus fields, method calls, routing, and response helpers have required guided examples. Continue practising these without copying a complete solution. Static lifetime and readonly are understood at an introductory level and need further application. Full dependency injection, async programming, and request-lifecycle understanding remain unassessed.

**Next action at that time:** GET the actual Location URL from the successful POST while the same application process is running. Then begin input validation.

## Study diary / 学习日记

Repository hygiene (14 October): removed all 36 tracked `obj` files from the index and local `main` history, preserving local build files and uncommitted work. The existing `.gitignore` is now committed and excludes future `obj` files. GitHub history has not been rewritten; remote-tracking refs and recovery references still retain the old history. Existing tracked `bin` files were outside this cleanup's scope.

Each daily file in [Diary/](Diary/) uses interview questions followed by reference answers, preserving English and Chinese explanations and code examples. Historical session records are kept separate from the answers.

[DIARY.md](DIARY.md) indexes the current learning notes and next understanding check.

学习笔记按天保存在 [Diary 文件夹](Diary/) 中，采用“面试问题 → 参考答案”格式，保留中英文解释和代码示例；历史进度记录单独列出。

- [Week 1 — Day 1 / 第 1 周 — 第 1 天 — 2026-10-08](Diary/2026-10-08.md)
- [Week 1 — Day 2 / 第 1 周 — 第 2 天 — 2026-10-09](Diary/2026-10-09.md)
- [Week 1 — Day 3 / 第 1 周 — 第 3 天 — 2026-10-10](Diary/2026-10-10.md)
- [Week 1 — Day 4 / 第 1 周 — 第 4 天 — 2026-10-11](Diary/2026-10-11.md)
- [Constructor injection and shared storage / 构造函数注入与共享存储 — 2026-10-15](Diary/2026-10-15.md)
- [Service CRUD and validation — interview Q&A / 服务 CRUD 与验证面试问答 — 2026-10-13](Diary/2026-10-13.md)
- [Git obj cleanup — interview Q&A / Git obj 清理面试问答 — 2026-10-14](Diary/2026-10-14.md)

The snapshot above was recorded on 9 October. A historical four-week plan appears in [Week 1, Day 3](Diary/2026-10-10.md); the current instructions specify the eight-week roadmap. We remain in Week 1 until foundational understanding is demonstrated.
