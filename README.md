# Victorian Plumbing - Code Review Task

Thank you for taking the time to participate in this technical assessment!

This task is to evaluate some code that is not up to our standards and yet could be seen in the real world. The code in this task overall conforms to SOLID principles and it follows domain driven development correctly as we do it, so the overall architecture, responsibility of the assemblies and their content is correct.  However the data model and code is poorly designed and the execution has a range of immediate bugs and some bugs that won't be seen until a sequence of events occurs. 

We’ve designed this task to give you an opportunity to showcase your knowledge and help us understand your approach to evaluating and solving technical challenges. It’s also something we’ll discuss further during the interview if you proceed to that stage.

At Victorian Plumbing, we value clear thinking, problem-solving, and the ability to identify issues early to build robust solutions. This task is your chance to demonstrate how you approach thinking about code.

We’ll use the results of this task as an important part of our decision-making process. While we may consider other factors, this task will be the primary focus in deciding whether to invite you for an interview.

We’re excited to see what you find so good luck!

## Objective

Your task is to perform a code review of a simple solution built using C# and Entity Framework. The solution consists of three API endpoints for managing orders.  Imagine this is taking customer input from a basket or customer service system to create/update an order record at a simplified level.  This order record would then go to downstream systems to be processed.  This system would become the source of truth for orders and we may refer back to it later if there was a problem with an order down the line.

## Some notes on the technical details

This solution uses Entity Framework with the generic repository pattern which has been been done correctly in our view and we use this pattern in our production code.  If you are unaware of EF Core or the repository pattern then it may be worth looking up.  Overall you don't need to look too closely at the code in the DataAccess assembly but check the database diagram.

The key points to understand with EF are you can load entities using the repositories, the DbContext will track all the objects that have been loaded, when the data is changed in these objects the DbContext knows.  Each time you call unitOfWork.Save() it wraps the individual call to the database in a transaction and saves all tracked changes to the database within that call.

## What You Need to Do

1. **Download the code**:
   - The code is available to download from https://github.com/Victorian-Plumbing/OrdersCodeReview.git

2. **Take some time to read the solution thoroughly and feel free to run it**:    
   - Check the database document in the DataAccess Assembly - [Link](DataAccess/database-diagram.md)
   - Make sure you understand the database design and structure.  When you run it a database called example.sqlite can be found in the API assembly
   - Each endpoint works with the example swagger payloads   
   - Play with it, test it and check the *details*
   - We haven't provided unit tests or acceptance criteria, this is deliberate, so there is no need to comment on this. We believe a good dev should be able to read and reason about code by itself
   - Some classes and interfaces contain placeholders or trade offs to meet the needs of setting up the task but they are generally labelled

3. **Write a Code Review**:
   - If you find anything you like then tell us!  We like keeping things constructive
   - Write a document that reports on the code, you may use any format you like
   - If you spot any potential bugs then write up how the bug would occur - this is the most important part
   - Check the data structure of the database, are there any issues here - also very important
   - If you spot anything where you can refer to a resource or documentation then provide a link and show off your sources and knowledge
   - Also consider the resources we've provided in the job description about how we write code
   - Consider best practises
   - Most people who fail the task are not looking at how it executes.  Don't spend too much time on big picture stuff, we care about working quality implementations first

5. **What not to do**:
   - Don't spend ages in the dbcontext file.  For this task we don't care the much about the entity framework mappings or database seeding.  Reason about the implementation of the problem
   - Don't tell us that files are in the wrong assemblies or tell us about the domain driven development principles, this solution is broadly structured correctly in our opinion
   - Don't tell us about stylistic issues, it's not a positive trait to argue about whether if statements should use one line or braces or if semi-colons are in the wrong place.  I'm afraid we just don't care about this stuff that much!

4. **Time Limit**:
   - Please spend no more than a couple of hours on this task.
   - If you get stuck then that's okay, just send us what you've managed to do
   - Do not feel like you have to cover every element of the code, but make sure what you cover is appropriate to your level of experience

### Important Note:

- **Take your time**: There is no rush to complete this task.  
- **Do not let any recruiter pressure you**: We do not require this task to be completed immediately or by a deadline.  However we may end hiring without notice if we fill all of our intended spots.
- If you don’t have time tonight, that’s absolutely fine, another evening is just as good.  
- We understand that you have a life outside of the job hunt, and we respect that.  

We want you to be at your best when completing this task, so please work at a pace that suits you!

We’re looking forward to seeing your thoughts!