# Study Diary / 学习日记

Last updated / 最后更新：2026-10-10（Australia/Sydney）。

English and Simplified Chinese versions cover the same learning topics. Days are reconstructed from the chat dates: Day 1 = 8 October, Day 2 = 9 October, Day 3 = 10 October. Some overlapping topics are grouped by their main study day; these are not timed attendance records. Guided work and source review do not imply independent mastery or successful runtime tests.

英文与简体中文版本覆盖相同的学习主题。日期按聊天记录整理：第 1 天为 10 月 8 日，第 2 天为 10 月 9 日，第 3 天为 10 月 10 日。跨天主题按主要学习日归类，不代表精确的学习时长记录。有指导的实现与源码审查不等于独立掌握或运行测试通过。

## English version

### Week 1 — Day 1 (2026-10-08)

#### Reference types and object identity

- A class is a reference type. Assigning its instance to another variable copies the reference, not the object.
- Two variables referencing the same object observe changes to that object.
- Reassigning one variable to a new object does not change the other variable's reference or replace an object held in a collection.
- Reassignment changes which object the variable refers to; it does not mean the original object's memory address changed.

```csharp
var first = new List<string> { "A" };
var second = first;
second.Add("B"); // first.Count == 2
second = new List<string> { "C" };
// first.Count == 2; second.Count == 1
```

#### Lists, arrays, and interfaces

- List<T> is a concrete collection whose size can grow or shrink. T is the element type.
- Creating an empty list does not create any of its elements. Objects must be created and added separately or supplied in an initializer.
- Add inserts an item. Count is a property, so it is read without parentheses. Indexes start at zero.
- Arrays have a fixed length. Their elements can still be replaced.
- IEnumerable<T> describes a sequence that can be iterated over. It is an interface, so it cannot be instantiated directly.
- A List<Event> or Event[] can be returned through IEnumerable<Event> because each supports that interface. The underlying object remains its concrete type.
- A collection expression using square brackets needs a target type. An object initializer inside it still needs object creation, such as new Event followed by its property initializer.

```csharp
var names = new List<string>();
names.Add("Alex");
IEnumerable<string> sequence = names;
string[] array = ["Alex", "Sam"];
// new IEnumerable<string>() is invalid / 不能实例化接口
```

#### Fields, properties, methods, and access

- Class members without an explicit access modifier are private by default. Inaccessible members can produce CS0122.
- A field stores data directly. An auto-property with get and set uses compiler-generated backing storage and provides accessors for reading and assigning its value.
- Public fields and public properties are different. The default System.Text.Json configuration serializes public properties but ignores fields; this explained the empty JSON objects in the first implementation.
- Properties can compute values. DateTime.Now is a property and is read without parentheses; Guid.NewGuid() is a method and is called with parentheses.
- DateTime.Now returns the current local date and time. Using it in a static initializer captures that initialization time, not a new time for every request.
- The @ prefix permits a keyword as an identifier: @event is a variable name, not a special parameter-binding feature.

```csharp
// Inside a class / 在类中声明：
public string? Title { get; set; }

// Inside a method / 在方法中使用：
var now = DateTime.Now;
var id = Guid.NewGuid();
```

#### Application setup and routing

- The .NET SDK supplies project creation, build, and run tools. C# Dev Kit provides C# support in VS Code.
- Program.cs configures services and the HTTP request pipeline; the project file declares the target framework and dependencies.
- AddControllers registers framework services for controllers with the dependency injection container. MapControllers maps attribute-routed controller actions to endpoints.
- Registering controller services alone does not make controller URLs reachable.
- An endpoint URL combines the application's base address, controller route, and action route.
- In a controller route, [controller] is replaced with the class name minus the Controller suffix.
- A route name, such as Name = "GetEvent", is not a URL path segment.
- A template such as {id} captures a URL segment. The matching Guid parameter receives the converted value.
- Two GET actions with the same route are ambiguous; different route names alone do not distinguish incoming requests.
- Attribute constructor arguments come before named property assignments.

#### Daily review: possible issues and correct approaches

1. **Possible issue:** Trying to create an instance of an interface such as `IEnumerable<Event>`.

   **Correct approach:** Create a concrete collection, then expose it through the interface when callers only need iteration.

   ```csharp
   var events = new List<Event>();
   IEnumerable<Event> result = events;
   ```

2. **Possible issue:** Treating object mutation and variable reassignment as the same operation.

   **Correct approach:** Mutation changes the shared object; reassignment changes only which object one variable references.

   ```csharp
   var first = new List<string> { "A" };
   var second = first;
   second.Add("B");                   // first also sees "B"
   second = new List<string> { "C" }; // first still references the old list
   ```

3. **Possible issue:** Confusing fields, properties, and methods.

   **Correct approach:** Use public properties for API models; read properties without parentheses and call methods with parentheses.

   ```csharp
   public string? Title { get; set; }

   var now = DateTime.Now;  // property
   var id = Guid.NewGuid(); // method
   ```

4. **Possible issue:** Treating a route name as part of the URL.

   **Correct approach:** The route template matches the URL; `Name` identifies the route for URL generation.

   ```csharp
   [HttpGet("{id}", Name = "GetEvent")]
   // URL shape: /events/{id}; route name: GetEvent
   ```

#### High-frequency interview questions and reference answers

**Q: What is the difference between a value type and a reference type?**  
**Reference answer:** A value-type variable contains its value. A reference-type variable contains a reference to an object. Assigning a reference copies the reference, so both variables may observe mutations to the same object; reassigning one variable does not reassign the other.

**Q: Why can a method return `IEnumerable<Event>` but not instantiate it?**  
**Reference answer:** `IEnumerable<Event>` is an interface contract, not a concrete implementation. A `List<Event>` or `Event[]` implements that contract and can therefore be returned through the interface type.

**Q: What is the difference between `AddControllers()` and `MapControllers()`?**  
**Reference answer:** `AddControllers()` registers controller-related framework services in dependency injection. `MapControllers()` adds attribute-routed controller endpoints to the request pipeline. Without mapping, requests do not reach controller actions.

### Week 1 — Day 2 (2026-10-09)

#### Static and readonly

- A static field belongs to the type and is shared across its instances. Our static collection initializer runs once for that type in the running application.
- A local collection created inside an action is recreated each time execution reaches that declaration.
- readonly restricts reassignment of a field after initialization. Assignment is allowed in its declaration or the appropriate constructor; a static readonly field can be assigned in a static constructor.
- For a reference type, readonly does not freeze the referenced object. Items can be added to a readonly List<Event>, and event properties can still change.
- Removing readonly does not make a static initializer run on every request.
- Neither static nor readonly makes List<T> safe for simultaneous reads and writes.
- In-memory data is lost when the application restarts. Seed event IDs are regenerated on startup.

```csharp
// Class field / 类字段：
private static readonly List<string> Names = new();

// Inside an ordinary method / 在普通方法中：
Names.Add("Alex");           // Allowed / 允许
// Names = new List<string>(); // Not allowed / 不允许重新赋值
```

#### Where request parameters come from

| Source | Attribute | Example |
|---|---|---|
| JSON request body | FromBody | POST /events with event JSON |
| URL path | FromRoute | /events/{id} |
| Query string | FromQuery | /events?location=syd |

With ApiController, a complex parameter such as Event is normally inferred from the body. FromBody makes this explicit. Binding input into an object does not establish that its business values are valid.

#### Lookups and HTTP results

- First returns the first matching item and throws if none matches.
- FirstOrDefault returns the first match or the element type's default value. For the Event class, that default is null.
- FirstOrDefault fits a lookup where an absent event is an expected outcome that should become 404.
- Neither method detects duplicate matching IDs.
- ActionResult<Event> supports either an event value or an HTTP action result such as NotFound().
- GET requests should retrieve data without changing business state. The temporary capacity-changing experiment was removed.

| Status | Meaning in this project |
|---|---|
| 200 OK | Retrieval succeeded |
| 201 Created | A new event was created |
| 404 Not Found | The requested event or endpoint was not found |

```csharp
var names = new List<string> { "Alex", "Sam" };
var missing = names.FirstOrDefault(n => n == "Jo"); // null
// names.First(n => n == "Jo"); // Throws / 抛出异常
```

#### Creating an event and CreatedAtAction

- The intended sequence is to generate the ID on the server, store the event, then construct the response. Validation must be added before storage.
- CreatedAtAction is a method; returning its name without calling it does not create a response.
- The overload used here takes an action name, route values, and a response-body object.
- nameof produces the action method's name as a string. Route values can be supplied using an anonymous object whose property name matches the route parameter, such as id.
- CreatedAtAction returns a CreatedAtActionResult. ASP.NET Core executes it to produce 201, a Location header, and the supplied body serialized as JSON.
- It does not store the event or invoke the GET action. It uses routing information to generate the resource URL.
- A client can GET the exact Location URL to retrieve the created resource.

```csharp
// Controller fragment; newEvent has already been stored.
// 控制器片段；newEvent 已加入存储。
return CreatedAtAction(
    nameof(GetEventById),
    new { id = newEvent.Id },
    newEvent);
```

#### Manual testing with curl

- Keep the API running and use a second terminal to send requests.
- Use the actual base address printed by dotnet run.
- curl -i includes response headers. -H sets a request header. -d supplies a request body and makes curl use POST by default.
- Send JSON with Content-Type: application/json.
- Opening a URL in the browser normally sends GET; it does not test a JSON POST action.
- Check status, headers, and body together. A successful POST response alone does not prove that its Location URL can retrieve the event.
- The confirmed POST test returned 201, a generated GUID, a Location URL containing that GUID, and the submitted title, location, start time, and capacity.

#### Daily review: possible issues and correct approaches

1. **Possible issue:** Assuming `readonly` makes a collection immutable or controls how often it is initialized.

   **Correct approach:** `static` provides one field per type; `readonly` prevents replacing the field reference after initialization. The referenced list can still change.

   ```csharp
   private static readonly List<Event> Events = new();

   Events.Add(new Event());      // allowed
   // Events = new List<Event>(); // not allowed outside initialization
   ```

2. **Possible issue:** Using `First()` when “not found” is a normal API outcome.

   **Correct approach:** Use `FirstOrDefault()` and explicitly convert `null` into a 404 response.

   ```csharp
   var eventItem = events.FirstOrDefault(e => e.Id == id);
   if (eventItem is null)
       return NotFound();
   ```

3. **Possible issue:** Returning the `CreatedAtAction` method name or supplying only an ID.

   **Correct approach:** Call the method with the lookup action name, named route values, and created response object.

   ```csharp
   return CreatedAtAction(
       nameof(GetEventById),
       new { id = newEvent.Id },
       newEvent);
   ```

4. **Possible issue:** Assuming a `201 Created` response proves the object was stored.

   **Correct approach:** Add the object before returning, then follow the `Location` header and verify GET returns it.

#### High-frequency interview questions and reference answers

**Q: What do `static` and `readonly` mean when used together?**  
**Reference answer:** `static` means the field belongs to the type and is shared across instances. `readonly` prevents reassignment after initialization. It does not make the referenced object immutable or thread-safe.

**Q: When should you use `First()` versus `FirstOrDefault()`?**  
**Reference answer:** Use `First()` when absence violates an invariant and should throw. Use `FirstOrDefault()` when absence is expected and the caller will handle the default value, such as returning 404 from an API.

**Q: What does `CreatedAtAction()` do?**  
**Reference answer:** It creates a 201 response containing a `Location` header generated from an action route and an optional response body. It does not persist data or invoke the target action.

### Week 1 — Day 3 (2026-10-10)

#### Validation and guard clauses

- Reject null, empty, or whitespace-only titles and locations, non-positive capacity, and start times that are not in the future.
- IsNullOrWhiteSpace is a method returning bool; call it with the string being checked. IsNullOrEmpty does not reject whitespace-only input.
- A guard clause returns early. Once the action returns, subsequent validation and storage code do not run.
- OR (||) rejects when either invalid condition is true; AND (&&) would require both conditions to be true.
- Separate checks give accurate field-specific messages. Validate everything before modifying stored data.
- BadRequest(...) creates a 400 response; BadRequestResult is a type, not a response instance.

```csharp
// Inside a controller action / 控制器操作方法内部：
if (string.IsNullOrWhiteSpace(request.Title))
    return BadRequest("Title is required.");

if (request.Capacity <= 0)
    return BadRequest("Capacity must be greater than zero.");
// Change stored data only after validation.
// 全部验证通过后才能修改存储数据。
```

#### CRUD responses and resource identity

- PUT /events/{id} selects the event using the route ID, preserves its ID, and returns 200 with the updated event. A missing event returns 404.
- DELETE /events/{id} removes the event and returns 204 with no body; a missing event returns 404.
- List.Remove returns bool, not an HTTP action result. Removal and response construction are separate operations.
- IActionResult is an interface for action results. ActionResult is a base class implementing it. ActionResult<T> represents either typed data or an ActionResult.
- Use ActionResult<Event> for a successful event body and IActionResult for deletion without a successful response body.
- A {id:guid} constraint only matches valid GUID route segments. Malformed IDs normally produce 404 for that route; an unconstrained Guid parameter under ApiController normally yields 400 on conversion failure.

#### Why IActionResult, ActionResult, or ActionResult<T>?

| Type | What it declares | Typical choice |
|---|---|---|
| IActionResult | Interface for an executable HTTP action result | DELETE returning 204 or 404 |
| ActionResult | Base class implementing IActionResult | Framework results derived from that class |
| ActionResult<T> | Either typed data T or an ActionResult | GET/PUT returning Event or an error result |

IActionResult can also return a body through Ok(value); it is not limited to bodyless responses. ActionResult<T> makes the successful body type explicit and helps API documentation. Non-generic ActionResult does not declare a body type. The return type alone does not choose the status code.

~~~csharp
// Illustrative controller method signatures, not complete methods:
public ActionResult<Event> GetEventById(Guid id) // Event body or 404
public IActionResult DeleteEvent(Guid id)       // 204 or 404

// Inside an ActionResult<Event> action:
return existingEvent;     // Typed value, normally 200
// Alternatively:
return Ok(existingEvent); // Explicit 200 with a body
// Missing-resource branch:
return NotFound();        // 404; valid for either signature above
~~~

Rule of thumb: use ActionResult<T> for a known success payload; use IActionResult when no success payload is needed or results have different shapes. ActionResult<Event> does not enforce every response body's type: Ok(...) accepts object, so returning the wrong DTO through it can still compile.

#### Request DTOs and mapping

- EventRequest lives in Contracts and defines client-editable fields: Title, Location, StartTime, and Capacity. It has no Id.
- Event remains the stored model. A request DTO and a stored event are different objects with different responsibilities.
- POST validates the request, constructs an Event, generates its ID, adds it to storage, and returns the created event rather than the request DTO.
- Returning 201 does not prove storage happened. A missing Add caused a creation response whose Location could not retrieve the event; this was corrected in source.
- PUT uses the route ID and copies validated editable values to the existing event. It must not use a body ID to choose another event.
- Trim removes leading and trailing whitespace. The null-forgiving operator (!) only suppresses compiler warnings; it performs no runtime validation.

#### Dates, offsets, and remaining limitations

- Event and EventRequest now use DateTimeOffset, representing a date/time with an offset. Use explicit offsets in requests and compare instants consistently against DateTimeOffset.UtcNow.
- DateTimeOffset carries an offset, not a complete named time zone or its daylight-saving rules.
- POST still used DateTime.Now at the latest review. It is implicitly converted for the DateTimeOffset comparison; standardising on UtcNow remains a clarity improvement.
- Seed times still represent application startup rather than future sample events. Shared List<Event> access remains unsafe under concurrent reads/writes.

```csharp
var sydney = new DateTimeOffset(
    2026, 11, 20, 18, 0, 0, TimeSpan.FromHours(11));
var utc = new DateTimeOffset(
    2026, 11, 20, 7, 0, 0, TimeSpan.Zero);

bool sameInstant = sydney == utc; // true / 同一时间点
```

#### Plan and evidence

- Replaced the original eight-week schedule with four weeks: backend/database; secure booking; React frontend; deployment and portfolio.
- Budget: 20 project hours plus 5 practice hours per week. Work in feature-sized batches with consolidated reviews.
- Target mid-to-senior full-stack roles, but do not equate guided implementation with independent mastery or senior readiness.
- Source review confirms CRUD, request DTO integration, validation-before-mutation, and the latest POST storage/body fixes. The full runtime CRUD flow has not been demonstrated in chat.
- Next assignment: extract EventRequestValidator into Validation. Accept a request and an explicit current time; return ordinary validation errors without HTTP results, mutations, or storage access. Reuse it in POST and PUT.
- Verify create → retrieve → update → retrieve → delete → retrieve, expecting 201 → 200 → 200 → 200 → 204 → 404, plus an invalid update that preserves stored values. Then introduce an injected event service and address storage lifetime.

#### Daily review: possible issues and correct approaches

1. **Possible issue:** Combining unrelated validation rules and returning a misleading message.

   **Correct approach:** Use separate guard clauses and complete validation before changing stored state.

   ```csharp
   if (string.IsNullOrWhiteSpace(request.Title))
       return BadRequest("Title is required.");

   if (request.Capacity <= 0)
       return BadRequest("Capacity must be greater than zero.");
   ```

2. **Possible issue:** Selecting an update target using an ID supplied in the request body.

   **Correct approach:** Let the route ID identify the resource. Exclude ID from the request DTO and preserve the stored entity ID.

   ```csharp
   var existingEvent = events.FirstOrDefault(e => e.Id == id);
   if (existingEvent is null)
       return NotFound();
   ```

3. **Possible issue:** Returning `List.Remove()` directly or returning 201 for an update.

   **Correct approach:** Treat collection mutation and HTTP response construction as separate operations. Successful update returns 200; successful deletion without a body returns 204.

   ```csharp
   events.Remove(existingEvent);
   return NoContent();
   ```

4. **Possible issue:** Constructing a created event but forgetting to add it, or returning the input DTO instead of the created resource.

   **Correct approach:** Validate, map the DTO, generate the server-owned ID, store the event, then return that stored event.

5. **Possible issue:** Comparing offset-aware request values using inconsistent clock conventions.

   **Correct approach:** Use `DateTimeOffset` consistently and compare instants against `DateTimeOffset.UtcNow`.

   ```csharp
   if (request.StartTime <= DateTimeOffset.UtcNow)
       return BadRequest("Start time must be in the future.");
   ```

#### High-frequency interview questions and reference answers

**Q: When should you use `IActionResult`, `ActionResult`, or `ActionResult<T>`?**  
**Reference answer:** Use `ActionResult<T>` when success returns a known data type but failures return HTTP results. Use `IActionResult` when no typed success body is needed or response shapes vary. `ActionResult` is the non-generic base class for many framework results. The declared return type does not choose the status code by itself.

**Q: Why use a request DTO instead of accepting the entity directly?**  
**Reference answer:** A request DTO defines the client-editable contract, prevents over-posting server-controlled fields such as ID, separates API input from persistence concerns, and allows the contract and entity to evolve independently.

**Q: How do you ensure an invalid update does not partially change data?**  
**Reference answer:** Validate the complete request before mutating the stored entity. For database-backed operations, apply changes and save within an appropriate transaction or unit of work so failure does not leave partial state.

**Q: Why use `DateTimeOffset` for event times?**  
**Reference answer:** It represents a specific date and time together with its UTC offset, so values from different offsets can be compared as instants. It does not replace a named time zone when future daylight-saving rules matter.

## 中文版

### 第 1 周 — 第 1 天 | 2026-10-08

#### 引用类型与对象身份

- class 是引用类型。把对象赋给另一个变量时，复制的是引用，不是对象本身。
- 两个变量引用同一个对象时，通过任一变量修改对象，另一个变量都能看到变化。
- 给其中一个变量重新赋一个新对象，不会改变另一个变量的引用，也不会替换集合中的原对象。重新赋值改变的是变量引用的对象，而不是原对象的内存地址。

```csharp
var first = new List<string> { "A" };
var second = first;
second.Add("B"); // first.Count == 2
second = new List<string> { "C" };
// first.Count == 2; second.Count == 1
```

#### 列表、数组与接口

- List<T> 是可增加或删除元素的具体集合类型，T 表示元素类型。创建空列表不会自动创建其中的对象。
- 对象需要单独创建并加入集合，也可以通过初始化器提供。Add 添加元素；Count 是属性，读取时不加括号；索引从 0 开始。
- 数组长度固定，但数组中的元素可以被替换。
- IEnumerable<T> 表示可遍历的序列，是接口，不能直接实例化。List<Event> 和 Event[] 都可以通过 IEnumerable<Event> 返回，底层对象仍然是原来的具体类型。
- 使用方括号的集合表达式需要目标类型；内部对象初始化器仍然需要创建对象，例如先写 new Event，再初始化属性。

```csharp
var names = new List<string>();
names.Add("Alex");
IEnumerable<string> sequence = names;
string[] array = ["Alex", "Sam"];
// new IEnumerable<string>() is invalid / 不能实例化接口
```

#### 字段、属性、方法与访问权限

- 类中未声明访问修饰符的成员默认为 private；外部访问可能产生 CS0122。
- 字段直接存储数据；带 get 和 set 的自动属性由编译器生成后备存储，并提供读取和赋值访问器。
- public 字段与 public 属性不同。默认 System.Text.Json 会序列化公开属性，但忽略字段，这解释了最初返回空 JSON 对象的问题。
- 属性也可以计算值。DateTime.Now 是属性，读取时不用括号；Guid.NewGuid() 是方法，调用时需要括号。
- DateTime.Now 返回本地当前时间；在静态初始化器中使用时，记录的是初始化时刻，不是每次请求的时刻。
- @ 前缀允许把关键字作为标识符使用。@event 只是变量名，不是参数绑定功能。

```csharp
// Inside a class / 在类中声明：
public string? Title { get; set; }

// Inside a method / 在方法中使用：
var now = DateTime.Now;
var id = Guid.NewGuid();
```

#### 项目设置与路由

- .NET SDK 提供项目创建、构建和运行工具；C# Dev Kit 为 VS Code 提供 C# 支持。
- Program.cs 配置服务和 HTTP 请求管道；项目文件声明目标框架和依赖。
- AddControllers 向依赖注入容器注册控制器所需的服务；MapControllers 把使用路由特性的控制器操作映射为端点。仅注册服务不会让 URL 可访问。
- 端点 URL 由应用基础地址、控制器路由和操作路由组成。[controller] 会替换为去掉 Controller 后缀的类名。
- Name = "GetEvent" 是路由名称，不是 URL 路径。
- {id} 捕获 URL 路径段，并转换后绑定到同名 Guid 参数。
- 两个 GET 操作使用相同路由会产生歧义；只改路由名称不能区分请求。
- 特性中的构造函数参数应放在命名属性赋值之前。

#### 每日复盘：可能的问题与正确写法

1. **可能的问题：**尝试直接创建 `IEnumerable<Event>` 接口实例。

   **正确写法：**创建具体集合；调用方只需要遍历时，再通过接口类型暴露。

   ```csharp
   var events = new List<Event>();
   IEnumerable<Event> result = events;
   ```

2. **可能的问题：**把修改对象和变量重新赋值理解成同一个操作。

   **正确写法：**修改对象会被所有共享该引用的变量看到；重新赋值只改变其中一个变量引用的对象。

   ```csharp
   var first = new List<string> { "A" };
   var second = first;
   second.Add("B");                   // first 也能看到 "B"
   second = new List<string> { "C" }; // first 仍引用原列表
   ```

3. **可能的问题：**混淆字段、属性和方法。

   **正确写法：**API 模型使用公开属性；属性读取不加括号，方法调用需要括号。

   ```csharp
   public string? Title { get; set; }

   var now = DateTime.Now;  // 属性
   var id = Guid.NewGuid(); // 方法
   ```

4. **可能的问题：**把路由名称当成 URL 的一部分。

   **正确写法：**路由模板负责匹配 URL，`Name` 用于标识路由及生成 URL。

   ```csharp
   [HttpGet("{id}", Name = "GetEvent")]
   // URL 结构：/events/{id}；路由名称：GetEvent
   ```

#### 高频面试题与参考回答

**问：值类型与引用类型有什么区别？**  
**参考回答：**值类型变量直接包含值；引用类型变量包含对象引用。引用赋值复制引用，因此两个变量可能看到同一个对象的变化；给其中一个变量重新赋值不会影响另一个变量。

**问：为什么方法可以返回 `IEnumerable<Event>`，却不能直接实例化它？**  
**参考回答：**`IEnumerable<Event>` 是接口契约，不是具体实现。`List<Event>` 或 `Event[]` 实现了这个接口，因此可以通过接口类型返回。

**问：`AddControllers()` 与 `MapControllers()` 有什么区别？**  
**参考回答：**`AddControllers()` 向依赖注入容器注册控制器所需的框架服务；`MapControllers()` 把特性路由控制器映射到请求管道。没有映射时，请求无法到达控制器操作。

### 第 1 周 — 第 2 天 | 2026-10-09

#### static 与 readonly

- static 字段属于类型，由该类型的实例共享。当前静态集合初始化器在运行中的应用内对该类型执行一次。
- 操作方法内的局部集合，在每次执行到其声明时都会重新创建。
- readonly 限制字段初始化后的重新赋值；允许在声明或相应构造函数中赋值，static readonly 对应静态构造函数。
- readonly 不会冻结引用的对象：仍可向列表添加元素、修改事件属性。移除 readonly 不会让 static 初始化器每次请求都执行。
- static 和 readonly 都不能保证 List<T> 的并发读写安全。应用重启会丢失内存数据，并重新生成种子事件 ID。

```csharp
// Class field / 类字段：
private static readonly List<string> Names = new();

// Inside an ordinary method / 在普通方法中：
Names.Add("Alex");           // Allowed / 允许
// Names = new List<string>(); // Not allowed / 不允许重新赋值
```

#### 请求参数来自哪里

| 来源 | 特性 | 示例 |
|---|---|---|
| JSON 请求体 | FromBody | POST /events，携带事件 JSON |
| URL 路径 | FromRoute | /events/{id} |
| 查询字符串 | FromQuery | /events?location=syd |

在 ApiController 下，Event 这样的复杂参数通常会被推断为来自请求体；FromBody 显式声明来源。绑定成功并不代表数据符合业务规则。

#### 查询与 HTTP 返回结果

- First 返回首个匹配项，没有匹配项时抛出异常。
- FirstOrDefault 返回首个匹配项或元素类型的默认值。Event 是类，因此默认值是 null；可以据此返回 404。
- 两个方法都不会检测重复 ID。
- ActionResult<Event> 可以返回事件值，也可以返回 NotFound() 等操作结果。
- GET 应读取数据而不修改业务状态；之前修改容量的实验代码已移除。
- 200 表示读取成功，201 表示资源已创建，404 表示资源或端点不存在。

```csharp
var names = new List<string> { "Alex", "Sam" };
var missing = names.FirstOrDefault(n => n == "Jo"); // null
// names.First(n => n == "Jo"); // Throws / 抛出异常
```

#### 创建事件与 CreatedAtAction

- 当时的创建顺序是服务器生成 ID、存储事件、构造响应；输入验证需要放在存储之前。
- CreatedAtAction 是方法，必须调用；只返回方法名不会创建响应。
- 使用的重载接收操作名称、路由值和响应对象。nameof 得到方法名称字符串；匿名对象可提供与路由参数同名的 id 属性。
- CreatedAtAction 返回 CreatedAtActionResult，由 ASP.NET Core 执行后产生 201、Location 响应头以及通常为 JSON 的响应体。
- 它不会存储事件，也不会调用 GET 操作，只利用路由信息生成 URL。客户端向 Location 中的完整 URL 发送 GET 即可查询资源。

```csharp
// Controller fragment; newEvent has already been stored.
// 控制器片段；newEvent 已加入存储。
return CreatedAtAction(
    nameof(GetEventById),
    new { id = newEvent.Id },
    newEvent);
```

#### 使用 curl 手动测试

- 保持 API 运行，在第二个终端发送请求，使用 dotnet run 输出的实际地址。
- curl -i 显示响应头；-H 设置请求头；-d 提供请求体，默认使 curl 使用 POST。JSON 请求使用 Content-Type: application/json。
- 浏览器直接打开 URL 通常发送 GET，不能代替 JSON POST 测试。
- 同时检查状态码、响应头和响应体。POST 成功不能单独证明 Location 能查询到资源。
- 已分享的 POST 结果包含 201、生成的 GUID、包含该 GUID 的 Location，以及提交的标题、地点、时间和容量。后续 GET 验证尚未分享。

#### 每日复盘：可能的问题与正确写法

1. **可能的问题：**认为 `readonly` 会使集合不可修改，或者决定初始化次数。

   **正确写法：**`static` 表示每个类型共享一个字段；`readonly` 只阻止初始化后替换字段引用，列表内容仍能变化。

   ```csharp
   private static readonly List<Event> Events = new();

   Events.Add(new Event());      // 允许
   // Events = new List<Event>(); // 初始化后不允许重新赋值
   ```

2. **可能的问题：**资源不存在是正常结果时仍使用 `First()`。

   **正确写法：**使用 `FirstOrDefault()`，并把 `null` 明确转换为 404。

   ```csharp
   var eventItem = events.FirstOrDefault(e => e.Id == id);
   if (eventItem is null)
       return NotFound();
   ```

3. **可能的问题：**返回 `CreatedAtAction` 方法名，或只传入一个 ID。

   **正确写法：**调用该方法，并提供查询操作名称、带名称的路由值和创建后的响应对象。

   ```csharp
   return CreatedAtAction(
       nameof(GetEventById),
       new { id = newEvent.Id },
       newEvent);
   ```

4. **可能的问题：**认为返回 `201 Created` 就证明对象已经存储。

   **正确写法：**先把对象加入存储，再访问 `Location` 响应头中的 URL，确认 GET 能查询到它。

#### 高频面试题与参考回答

**问：`static` 与 `readonly` 同时使用时分别表示什么？**  
**参考回答：**`static` 表示字段属于类型并由所有实例共享；`readonly` 表示初始化后不能重新赋值。它不会让引用对象不可变，也不会保证线程安全。

**问：什么时候用 `First()`，什么时候用 `FirstOrDefault()`？**  
**参考回答：**当没有匹配项代表程序不变量被破坏、应该抛异常时用 `First()`；当不存在是正常业务结果，并由调用者处理默认值时用 `FirstOrDefault()`，例如 API 返回 404。

**问：`CreatedAtAction()` 做什么？**  
**参考回答：**它创建一个 201 响应，使用目标操作的路由生成 `Location` 响应头，并可包含响应体。它不会持久化数据，也不会调用目标操作。

### 第 1 周 — 第 3 天 | 2026-10-10

#### 验证与提前返回

- 拒绝 null、空字符串或纯空白的标题和地点，拒绝小于等于 0 的容量，以及不在未来的开始时间。
- IsNullOrWhiteSpace 是返回 bool 的方法，需要传入要检查的字符串。IsNullOrEmpty 不会拒绝纯空白字符串。
- Guard clause（保护性检查）通过提前 return 结束方法，后续验证和存储逻辑不会执行。
- || 表示任一无效条件成立就拒绝；&& 则要求两个无效条件同时成立。
- 分开的验证可以提供准确的字段错误信息。必须先完成全部验证，再修改已存储数据。
- BadRequest(...) 创建 400 响应；BadRequestResult 是类型名称，不是响应实例。

```csharp
// Inside a controller action / 控制器操作方法内部：
if (string.IsNullOrWhiteSpace(request.Title))
    return BadRequest("Title is required.");

if (request.Capacity <= 0)
    return BadRequest("Capacity must be greater than zero.");
// Change stored data only after validation.
// 全部验证通过后才能修改存储数据。
```

#### CRUD 响应与资源身份

- PUT /events/{id} 使用路由 ID 查找事件，保留原 ID，返回 200 和更新后的事件；不存在时返回 404。
- DELETE /events/{id} 删除事件后返回无响应体的 204；不存在时返回 404。
- List.Remove 返回 bool，不是 HTTP 操作结果。删除操作与构造响应是两件事。
- IActionResult 是操作结果接口；ActionResult 是实现它的基类；ActionResult<T> 可表示指定类型的数据或 ActionResult。
- 成功时返回事件可使用 ActionResult<Event>；删除成功没有响应体，可使用 IActionResult。
- {id:guid} 只匹配合法 GUID 路径段，格式错误通常导致该路由返回 404；在 ApiController 下，没有此约束的 Guid 参数转换失败通常返回 400。

#### 为什么有时用 IActionResult，有时用 ActionResult？

| 类型 | 声明的含义 | 常见选择 |
|---|---|---|
| IActionResult | 可执行 HTTP 操作结果的接口 | DELETE 返回 204 或 404 |
| ActionResult | 实现 IActionResult 的基类 | 使用继承自该类的框架结果 |
| ActionResult<T> | T 类型的数据或 ActionResult | GET/PUT 返回 Event 或错误结果 |

IActionResult 也能通过 Ok(value) 返回响应体，并非只能返回无响应体的结果。ActionResult<T> 明确成功响应的数据类型，也有助于生成 API 文档。非泛型 ActionResult 不声明响应体类型。仅靠返回类型不会决定 HTTP 状态码。

~~~csharp
// 控制器方法签名示意，不是完整方法：
public ActionResult<Event> GetEventById(Guid id) // Event 响应体或 404
public IActionResult DeleteEvent(Guid id)       // 204 或 404

// 在返回 ActionResult<Event> 的方法中：
return existingEvent;     // 直接返回数据，通常为 200
// 另一种方式：
return Ok(existingEvent); // 显式返回 200 和响应体
// 资源不存在的分支：
return NotFound();        // 404；以上两种签名都支持
~~~

实用原则：成功响应有明确数据类型时用 ActionResult<T>；成功时没有响应体，或结果形状不同时可用 IActionResult。ActionResult<Event> 不会严格限制所有响应体类型：Ok(...) 接收 object，因此通过它返回错误的 DTO 仍可能编译通过。

#### 请求 DTO 与对象映射

- Contracts 中的 EventRequest 定义客户端可提交的 Title、Location、StartTime、Capacity，不包含 Id。
- Event 是存储模型，DTO 与存储对象职责不同。
- POST 验证请求后创建 Event，生成 ID，加入集合，再返回创建的事件，而不是请求 DTO。
- 201 不代表数据已存储。之前遗漏 Add 导致返回创建成功却无法通过 Location 查询，已在源码中修复。
- PUT 使用路由 ID，并把验证后的可编辑字段复制到原事件，不能用请求体 ID 选择另一个事件。
- Trim 去掉两端空白；空值抑制运算符 ! 只抑制编译器警告，不执行运行时验证。

#### 时间、偏移量与剩余限制

- Event 和 EventRequest 已使用 DateTimeOffset。请求应携带明确偏移量，并统一与 DateTimeOffset.UtcNow 比较时间点。
- DateTimeOffset 保存偏移量，不包含完整的命名时区及其夏令时规则。
- 最近审查时 POST 仍使用 DateTime.Now；比较时会隐式转换为 DateTimeOffset。统一使用 UtcNow 可提高一致性与可读性。
- 种子时间仍是启动时刻，不是未来的示例事件。共享 List<Event> 的并发读写安全仍未解决。

```csharp
var sydney = new DateTimeOffset(
    2026, 11, 20, 18, 0, 0, TimeSpan.FromHours(11));
var utc = new DateTimeOffset(
    2026, 11, 20, 7, 0, 0, TimeSpan.Zero);

bool sameInstant = sydney == utc; // true / 同一时间点
```

#### 计划与验证证据

- 原八周计划已调整为四周：后端与数据库、安全预订、React 前端、部署与作品集。
- 每周 20 小时项目开发加 5 小时专项练习；按完整功能批量实施和审查。
- 目标是中级到高级全栈岗位，但完成有指导的实现不等于独立掌握，也不能直接证明高级工程师能力。
- 源码审查确认了 CRUD、DTO 接入、先验证后修改，以及最近的 POST 存储与响应体修复；完整运行时 CRUD 流程尚未在聊天中展示。
- 下一任务：在 Validation 中提取 EventRequestValidator，接收请求及显式传入的当前时间，返回普通验证错误，不返回 HTTP 结果、不修改数据、不访问存储；POST 和 PUT 共用。
- 验证创建 → 查询 → 更新 → 查询 → 删除 → 查询，预期为 201 → 200 → 200 → 200 → 204 → 404，并检查无效更新不会改变原数据。随后学习注入事件服务及存储生命周期。

#### 每日复盘：可能的问题与正确写法

1. **可能的问题：**把无关的验证条件合并，导致返回误导性的错误信息。

   **正确写法：**为不同字段使用独立的保护性检查，并在修改存储状态前完成全部验证。

   ```csharp
   if (string.IsNullOrWhiteSpace(request.Title))
       return BadRequest("Title is required.");

   if (request.Capacity <= 0)
       return BadRequest("Capacity must be greater than zero.");
   ```

2. **可能的问题：**使用请求体中的 ID 选择要更新的资源。

   **正确写法：**由路由 ID 唯一确定资源；请求 DTO 不包含 ID，并保留已存储事件的 ID。

   ```csharp
   var existingEvent = events.FirstOrDefault(e => e.Id == id);
   if (existingEvent is null)
       return NotFound();
   ```

3. **可能的问题：**直接返回 `List.Remove()` 的结果，或更新时返回 201。

   **正确写法：**集合修改和 HTTP 响应是两个操作。更新成功返回 200；删除成功且无响应体时返回 204。

   ```csharp
   events.Remove(existingEvent);
   return NoContent();
   ```

4. **可能的问题：**创建事件对象后忘记加入存储，或返回请求 DTO 而不是创建后的资源。

   **正确写法：**先验证，再映射 DTO、生成服务器控制的 ID、存储事件，最后返回已存储的事件。

5. **可能的问题：**使用不一致的时间标准比较带偏移量的请求时间。

   **正确写法：**统一使用 `DateTimeOffset`，并与 `DateTimeOffset.UtcNow` 比较时间点。

   ```csharp
   if (request.StartTime <= DateTimeOffset.UtcNow)
       return BadRequest("Start time must be in the future.");
   ```

#### 高频面试题与参考回答

**问：什么时候使用 `IActionResult`、`ActionResult` 或 `ActionResult<T>`？**  
**参考回答：**成功时返回明确数据类型，而失败时返回 HTTP 结果，可使用 `ActionResult<T>`；成功时不需要类型化响应体，或不同分支返回不同形状，可使用 `IActionResult`；`ActionResult` 是许多框架结果的非泛型基类。返回类型本身不会决定状态码。

**问：为什么使用请求 DTO，而不是直接接收实体？**  
**参考回答：**请求 DTO 明确客户端允许编辑的字段，防止 ID 等服务器控制字段被过度提交，把 API 输入与持久化模型分离，并允许两者独立演进。

**问：怎样确保无效更新不会部分修改数据？**  
**参考回答：**必须先验证完整请求，再修改已存储实体。使用数据库时，应在合适的事务或工作单元中应用并保存更改，避免失败后留下部分状态。

**问：事件时间为什么使用 `DateTimeOffset`？**  
**参考回答：**它同时保存日期时间及 UTC 偏移量，因此不同偏移量的值可以作为时间点比较。如果业务需要未来的夏令时规则，仍然需要额外保存或使用命名时区。

## Shared references / 共用参考资料

- [C# type system](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/)
- [ASP.NET Core fundamentals](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/?view=aspnetcore-10.0)
- [C# in VS Code](https://code.visualstudio.com/docs/csharp/get-started)
- [Controller-based Web API tutorial](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api)
- [System.Text.Json field serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/fields)
