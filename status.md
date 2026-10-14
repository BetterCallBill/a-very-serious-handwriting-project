# Learning status

Last updated: 15 October 2026 (Australia/Sydney).

## Working agreement

Follow the current AGENTS.md and instructions supplied for this session:

- Complete requested implementation, configuration, and verification; minimize manual coding.
- Use independent exercises and hint-only teaching only when explicitly requested or accepted.
- Keep README.md, DIARY.md, this status file, and dated learning notes current.
- For review-only requests, preserve executable code and follow the REVIEW-comment rules.
- Do not award assessment points for assistant-generated code or advance beyond Week 1 without evidence of foundational understanding.

The template-only agreement recorded on 13 October is historical and superseded by the current instructions. Its session record remains in [Diary/2026-10-13.md](Diary/2026-10-13.md). The current roadmap is eight weeks.

## Current progress

- **Current week:** Week 1 — C# and ASP.NET Core fundamentals.
- **Current day:** Session of 15 October; service integration completed.
- **Completed tasks:** Existing CRUD, request DTO, validation, and service methods are now connected to every controller action through constructor injection. The existing singleton registration is used. One private lock protects service storage; all returned events are copies and GetAll materializes its snapshot inside the lock.
- **Current coding challenge:** Integration is complete. Next focus is the HTTP request lifecycle and the distinction between service lifetime, synchronization, and object references.
- **Concepts understood:** Earlier evidence supports the learner's explanation that repeated deletion returns false because the event no longer exists. Today's implementation and test results provide no new independent-understanding evidence.
- **Knowledge gaps:** DI lifetimes, controller/service responsibilities, snapshots and locks, middleware, exception handling, and async/await still need explanation and assessment.
- **Technical assessment scores:** Unchanged; no new scores awarded. Other roadmap areas remain unassessed as previously recorded.
- **Unresolved questions:** Why does per-request service creation lose instance-owned events between requests? Why are both locking and copies needed? What happens between receiving JSON and returning the response?
- **Known limitations:** Data disappears on restart and is private to one application process. Store operations serialize behind a lock. Stale client updates are last-write-wins. Authentication, persistence, and booking capacity rules are not implemented. Existing OpenAPI dependency warning remains a follow-up.
- **Next action:** Trace POST through routing, model binding, controller activation/injection, service validation, and serialization; continue middleware and async/await within Week 1. Optional recall question: would a later GET find a POST-created event if registration changed from singleton to scoped?

## Verification — 15 October

- API build succeeded with zero errors and one existing NU1903 warning for transitive Microsoft.OpenApi 2.0.0.
- Repeatable `scripts/verify-events.py` passed 46 HTTP checks covering CRUD, following Location, field trimming, ID preservation, invalid POST/PUT requests, unchanged storage after rejection, missing-resource precedence, 40 parallel creates, 60 concurrent update/read sequences, and 12 competing deletes with exactly one success.
- A temporary console harness compiled the actual service source and passed 14 direct checks for explicit validation time, equal-time rejection, independent returned objects from every method, validation-before-mutation, missing-ID precedence, materialized list snapshots, and empty-store behavior.
- Build output was directed to `/tmp` to avoid altering tracked bin files. The temporary HTTP server was stopped after testing.
- These checks provide regression evidence for the exercised cases; they do not constitute a load test or proof of distributed concurrency safety.

## Previous evidence and maintenance

The learner's Delete, Create, and Update methods passed sequential review on 13 October. Create passed 29 direct assertions; Update passed 50. See [13 October notes](Diary/2026-10-13.md) for review findings, corrections, and learning evidence.

Repository maintenance (14 October): removed 36 tracked obj files from the index and local main history, preserving local files. Existing tracked bin files and GitHub history were outside that cleanup. See [maintenance notes](Diary/2026-10-14.md).

Latest learning notes: [15 October — Constructor injection and shared storage](Diary/2026-10-15.md).
