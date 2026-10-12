# Event Booking & Ticketing Platform

## Current progress

Last updated: 12 October 2026 (Australia/Sydney).

- **Current stage:** Week 1 fundamentals; fourth recorded learning session.
- **Completed:** CRUD actions, request DTO, shared request validation, validation before mutation. Review on 12 October found no new defects in the validator extraction or service lookup methods. Local HTTP checks passed the complete CRUD sequence, trimming, ID preservation, 14 invalid POST/PUT requests, unchanged storage after rejected updates, and missing-resource PUT/DELETE responses.
- **Current exercise:** `EventService.GetAll` and `GetById`, with two sample events, passed source review; direct service runtime checks have not been run. The service is not yet registered or used by the controller. The API builds with zero errors and one NU1903 warning for transitive `Microsoft.OpenApi` 2.0.0; previous zero-warning records are historical.
- **Working preference:** Implement ordinary requested work with minimal manual coding. Review-only requests preserve executable code and provide commented corrections for findings; passing reviews remove comments from reviewed source files.
- **Understanding demonstrated:** See the historical record below. No new assessment scores have been awarded; implementation and build success alone do not establish independent understanding.
- **Knowledge gaps / pending checks:** Dependency injection and service lifetimes remain unassessed. Shared in-memory storage is not yet safe for concurrent access; the HTTP checks were sequential. Review the OpenAPI dependency warning before introducing untrusted document parsing (see [advisory](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)); no such input path was found in this API.
- **Next action:** Migrate the remaining service operations and introduce constructor injection with a deliberate storage lifetime. Keep all endpoints on one store during integration. Review collection copies versus shared event references before relying on storage encapsulation.
- **Unresolved question:** What happens to instance-owned events if a new service instance is created for every request?

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

Each daily file in [Diary/](Diary/) uses interview questions followed by reference answers, preserving English and Chinese explanations and code examples. Historical session records are kept separate from the answers.

学习笔记按天保存在 [Diary 文件夹](Diary/) 中，采用“面试问题 → 参考答案”格式，保留中英文解释和代码示例；历史进度记录单独列出。

- [Week 1 — Day 1 / 第 1 周 — 第 1 天 — 2026-10-08](Diary/2026-10-08.md)
- [Week 1 — Day 2 / 第 1 周 — 第 2 天 — 2026-10-09](Diary/2026-10-09.md)
- [Week 1 — Day 3 / 第 1 周 — 第 3 天 — 2026-10-10](Diary/2026-10-10.md)
- [Week 1 — Day 4 / 第 1 周 — 第 4 天 — 2026-10-12](Diary/2026-10-12.md)

The progress snapshot above was recorded on 9 October. The eight-week schedule was superseded by the agreed four-week plan on 10 October; see [Week 1, Day 3](Diary/2026-10-10.md).
