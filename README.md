# Event Booking & Ticketing Platform

## Current progress

Last updated: 13 October 2026 (Australia/Sydney). See [status.md](status.md) for the current task and working agreement.

- **Current stage:** Week 1 fundamentals; fourth recorded learning session.
- **Completed:** CRUD actions, request DTO, shared request validation, validation before mutation. Review on 12 October found no new defects in the validator extraction or service lookup methods. Local HTTP checks passed the complete CRUD sequence, trimming, ID preservation, 14 invalid POST/PUT requests, unchanged storage after rejected updates, and missing-resource PUT/DELETE responses.
- **Current exercise:** `EventService.Delete`, `Create`, and `Update` passed review and direct runtime checks on 13 October. Update passed 50 assertions, including missing IDs, 11 invalid cases, supplied time, trimming, ID preservation, and unchanged storage after rejected updates. The service is not yet registered or used by the controller. The previous API build reported one NU1903 warning for transitive `Microsoft.OpenApi` 2.0.0.
- **Working preference:** After a review passes, update `status.md` and add a diary entry. Provide templates and routine boilerplate; the learner writes the key lines and logic with guidance. This preference was explicitly corrected on 13 October and supersedes the earlier automatic implementation workflow.
- **Understanding demonstrated:** The learner correctly explained why repeated deletion returns false and implemented the corresponding lookup, null guard, and removal. No broader assessment scores were awarded.
- **Knowledge gaps / pending checks:** Dependency injection and service lifetimes remain unassessed. Shared in-memory storage is not yet safe for concurrent access; the HTTP checks were sequential. Review the OpenAPI dependency warning before introducing untrusted document parsing (see [advisory](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc)); no such input path was found in this API.
- **Next action:** Learn constructor injection and choose a service lifetime for instance-owned in-memory storage. Then integrate all controller actions with one service store and verify HTTP behavior. Concurrent access and shared mutable event references still need attention before treating this as reliable storage.
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

Repository hygiene (14 October): removed all 36 tracked `obj` files from the index and local `main` history, preserving local build files and uncommitted work. The existing `.gitignore` is now committed and excludes future `obj` files. GitHub history has not been rewritten; remote-tracking refs and recovery references still retain the old history. Existing tracked `bin` files were outside this cleanup's scope.

Each daily file in [Diary/](Diary/) uses interview questions followed by reference answers, preserving English and Chinese explanations and code examples. Historical session records are kept separate from the answers.

[DIARY.md](DIARY.md) indexes the current learning notes and next understanding check.

学习笔记按天保存在 [Diary 文件夹](Diary/) 中，采用“面试问题 → 参考答案”格式，保留中英文解释和代码示例；历史进度记录单独列出。

- [Week 1 — Day 1 / 第 1 周 — 第 1 天 — 2026-10-08](Diary/2026-10-08.md)
- [Week 1 — Day 2 / 第 1 周 — 第 2 天 — 2026-10-09](Diary/2026-10-09.md)
- [Week 1 — Day 3 / 第 1 周 — 第 3 天 — 2026-10-10](Diary/2026-10-10.md)
- [Week 1 — Day 4 / 第 1 周 — 第 4 天 — 2026-10-11](Diary/2026-10-11.md)
- [Workflow correction and deletion exercise / 学习流程调整与删除练习 — 2026-10-13](Diary/2026-10-13.md)

The progress snapshot above was recorded on 9 October. The eight-week schedule was superseded by the agreed four-week plan on 10 October; see [Week 1, Day 3](Diary/2026-10-10.md).
