# Learning status

Last updated: 13 October 2026 (Australia/Sydney).

## Working agreement

The user's instruction on 13 October supersedes the earlier automatic implementation preference for learning tasks:

- After a review passes, update this file and add a dated entry under `Diary/`.
- Generate scaffolding, signatures, and routine boilerplate only. Leave the important logic and key lines for the learner to write.
- Explain the objective, inputs, outputs, and acceptance criteria. Guide with questions and hints, then wait for the learner's implementation.
- Review the implementation before advancing. Preserve executable code during review; use REVIEW comments with suggested corrections according to the existing review rules.
- Do not complete the key implementation unless the learner explicitly asks for the solution.

## Current progress

- **Current week:** Week 1 — C# and ASP.NET Core fundamentals.
- **Current day:** 13 October; continuation of the service extraction task.
- **Completed:** Controller CRUD, request DTO, shared validation, and service `GetAll` / `GetById` passed the previous review. Learner-written `Delete(Guid id)` passed source review and runtime checks on 13 October; completed TODO comments were removed without changing executable logic.
- **Verification already recorded:** Earlier API build succeeded with one NU1903 dependency warning, and sequential HTTP CRUD checks passed. A temporary console harness compiled the actual service and verified existing, missing, repeated, and empty-store deletions, lookup/list consistency, and preservation of the other event's fields. No concurrency checks or new HTTP integration checks were performed during the Delete review.
- **Reverted:** Assistant-written service CRUD, locking, copies, dependency injection, clock injection, and test/solution scaffolding from the interrupted continuation. These are not learner achievements.
- **Current coding challenge:** `EventService.Update` is complete and passed review for sequential service behavior. Next is constructor injection and selecting a lifetime that preserves instance-owned events across requests. The controller still uses its original store.
- **Concepts understood:** The learner correctly explained that a second deletion returns false because the lookup finds no event, and implemented lookup, a null guard, and a boolean removal result. This supports the specific deletion concept, not a broad claim of C# mastery.
- **Knowledge gaps:** Service versus controller responsibilities, DI lifetimes, shared references, and concurrent storage access remain to be assessed.
- **Technical assessment scores:** Unchanged; no points awarded for assistant-generated code.
- **Unresolved questions:** Why must every update field be validated before modifying the stored event? How will all endpoints share the same store after integration?
- **Known limitations:** The original shared list remains unsafe under concurrency. `GetAll` copies the collection but shares its event objects. The existing OpenAPI dependency warning remains.
- **Latest review:** `Update` passed source review and a temporary console harness compiling the actual source. All 50 assertions passed: missing-ID precedence, 11 invalid cases with unchanged storage and request data, supplied historical time, trimmed/mapped fields, stable ID/count, retrieval/list consistency, unchanged other events, repeated updates, rejection after success, and an empty store. Removed completed template comments without changing executable logic. No HTTP integration, concurrency checks, or full API build were run in this review. Independent explanation remains unassessed; scores are unchanged.
- **Next action:** Explain what happens if each request receives a new `EventService`, then prepare the controller-integration exercise. Integrate every endpoint with the same store and verify create-followed-by-GET, update, deletion, and error responses; address shared storage safety as part of integration.

## Update acceptance criteria — passed

1. A missing ID returns `(null, null)` without changing storage, even when the request is invalid; this preserves the controller's existing lookup-first behavior.
2. An existing event with invalid input returns `(null, error)` and retains all original values; reuse the validator with the supplied `now`.
3. Success returns `(updatedEvent, null)`, preserves its ID, trims title/location, and applies the requested start time/capacity.
4. GetById/GetAll reflect the update; event count and other events remain unchanged. Return no HTTP results from the service.

## Latest diary

Repository maintenance (14 October): removed 36 tracked `obj` files from the index and all local `main` commits; preserved local files and uncommitted work. Committed `.gitignore` as `d5f295c`. GitHub history remains unchanged pending a decision about force-pushing; recovery refs and remote-tracking refs retain old history. Existing tracked `bin` files were not part of the requested cleanup. The current learning topic remains dependency injection and service lifetimes.

[13 October — Delete/Create/Update reviews and DI next step](Diary/2026-10-13.md)
