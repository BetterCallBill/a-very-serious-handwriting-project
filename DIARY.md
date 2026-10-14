# Learning diary

Daily notes remain in [Diary/](Diary/) with their existing English and Chinese explanations.

Latest maintenance: [14 October 2026 — Remove obj from local Git history](Diary/2026-10-14.md).

Latest learning session: [13 October 2026 — Update review passed](Diary/2026-10-13.md#update-review--passed--更新方法审查通过).

- Completed: Update passed source review and 50 direct service assertions. Only completed template comments were removed.
- Understanding: Implementation follows lookup, validation, then mutation. Independent explanation has not yet been demonstrated; no new scores assigned.
- Difficulties: No new defect found in this submission. DI lifetimes, shared references, and concurrency remain learning gaps.
- Revise: A lookup returns a reference to the stored Event; changing its properties changes that stored object. Returning an error does not undo earlier assignments.
- Next: Explain the effect of creating a new EventService for every HTTP request, then continue constructor injection and controller integration.

Current task and working agreement: [status.md](status.md).
