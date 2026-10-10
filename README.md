# Event Booking & Ticketing Platform

## 1. Current progress according to the 8-week plan

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

**Next action:** GET the actual Location URL from the successful POST while the same application process is running. Then begin input validation.

## Study diary / 学习日记

Study notes have moved to [DIARY.md](DIARY.md), grouped by week and day in English and Simplified Chinese.

学习笔记已移至 [DIARY.md](DIARY.md)，按周和天整理，提供英文和简体中文两个版本。

The progress snapshot above was recorded on 9 October. The eight-week schedule was superseded by the agreed four-week plan on 10 October; see Week 1, Day 3 in the diary.
