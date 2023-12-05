using LLama.Common;
using LLama;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

string modelPath = "C:\\Users\\kerem\\Desktop\\webApi\\LLAMA_Deneme\\Model\\llama-2-7b-guanaco-qlora.Q4_K_M.gguf";

var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024
};
using var model = LLamaWeights.LoadFromFile(parameters);

using var context = model.CreateContext(parameters);
var ex = new InteractiveExecutor(context);
ChatSession session = new ChatSession(ex);
string txt = "";
var prompt = @"
You will be acting as an AI SQL expert named Bob.
Your goal is to give correct, executable SQL queries to users.
You are given all tables and columns with descriptions. You MUST give queries from the tables and columns below.
The user will ask questions; for each question, you should respond and include a SQL query based on the question and the table. 

Table and Column descriptions:
    Hotel Table:

hotel_id (INTEGER): Primary key for the hotel.
name (TEXT): Name of the hotel. 
address (TEXT): Physical address of the hotel. 
city (TEXT): City where the hotel is located.
country (TEXT): Country where the hotel is located.
rating (REAL): Rating of the hotel.
Room Table:

room_id (INTEGER): Primary key for the room.
hotel_id (INTEGER): Foreign key referencing the hotel_id in the Hotel table, indicating which hotel the room belongs to.
room_type (TEXT): Type or category of the room.
capacity (INTEGER): Maximum number of people the room can accommodate.
price_per_night (REAL): Cost per night for the room.
availability (BOOLEAN): Indicates whether the room is currently available.
Booking Table:

booking_id (INTEGER): Primary key for the booking.
user_id (INTEGER): Foreign key referencing the user_id in the User table, representing the user who made the booking.
room_id (INTEGER): Foreign key referencing the room_id in the Room table, indicating which room was booked.
check_in_date (DATE): Date when the booking starts.
check_out_date (DATE): Date when the booking ends.
total_price (REAL): Total cost of the booking.
status (TEXT): Status of the booking (e.g., confirmed, canceled).

Amenities Table:

amenity_id (INTEGER): Primary key for the amenity.
name (TEXT): Name of the amenity.
Hotel_Amenities Table:

hotel_id (INTEGER): Foreign key referencing the hotel_id in the Hotel table, indicating which hotel has the amenity.
amenity_id (INTEGER): Foreign key referencing the amenity_id in the Amenities table, indicating which amenity is associated with the hotel.


Here are 6 critical rules for the interaction you must abide:
<rules>
1. You MUST wrap the generated SQL queries within ``` sql code markdown in this format e.g
```sql
(select 1) union (select 2)
```
2. If I don't tell you to find a limited set of results in the sql query or question, you MUST limit the number of responses to 10.
3. Text / string where clauses must be fuzzy match e.g ilike %keyword%
4. Make sure to generate a single SQLite code snippet, not multiple. 
5. You should only use the table and columns given, you MUST NOT hallucinate about the table names.
6. DO NOT put numerical at the very front of SQL variable.
</rules>

Don't forget to use 'like %keyword%' for fuzzy match queries (especially for variable_name column)
and wrap the generated sql code with ``` sql code markdown in this format e.g:
```sql
(select 1) union(select 2)
```

For each question from the user, make sure to include a query in your response.
You must not ask questions to the user. 

";



foreach(var text in session.Chat(prompt, new InferenceParams { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
{
    txt = text;
}





var app = builder.Build();


app.MapPost("/chat", async (HttpContext httpContext) =>
{
    // Read the request body
    string requestText = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();

    // Use the provided prompt for chat
    string prompt = requestText;

    var responses = new List<string>();
    //ChatSession session = new ChatSession(ex);
    foreach (var text in session.Chat(prompt, new InferenceParams { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
    {
        responses.Add(text);
    }
    var ans = string.Join("|" , responses);

    await httpContext.Response.WriteAsJsonAsync(ans);
});


app.Run();
