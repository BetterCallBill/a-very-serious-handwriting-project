# Learning diary

Daily notes remain in [Diary/](Diary/) with their existing English and Chinese explanations. The [13 October service CRUD notes](Diary/2026-10-13.md) and [14 October Git cleanup notes](Diary/2026-10-14.md) now use interview questions and reference answers, with historical session records preserved separately.

Latest learning session: [15 October 2026 — Constructor injection and shared storage](Diary/2026-10-15.md).

- Completed: Connected all controller actions to the existing singleton EventService; added synchronized storage access and independent response snapshots.
- Verification: API build passed; 46 repeatable HTTP checks including concurrent requests passed; 14 temporary direct service checks passed. Existing NU1903 dependency warning remains.
- Understanding: Today's implementation was assistant-generated. Previously demonstrated deletion reasoning remains recorded; no new assessment scores awarded.
- Difficulties: No new learner attempt was made, so no new learner difficulty can be inferred. DI lifetime, shared references, locking, and request lifecycle remain unassessed.
- Revise: Singleton controls object lifetime; a lock coordinates concurrent access; copies prevent later mutation through returned references. Each solves a different problem.
- Next: Trace the POST request lifecycle, explain scoped versus singleton storage, then continue middleware and async/await in Week 1.
- Optional understanding check: Would a later GET find a POST-created event if EventService were scoped? Explain using its instance-owned list.

Latest maintenance: [14 October 2026 — Remove obj from local Git history](Diary/2026-10-14.md).

Current task and working agreement: [status.md](status.md).
