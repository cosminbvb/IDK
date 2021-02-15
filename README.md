# IDK

This project was made as an assignment for the Web App Development Course (University of Bucharest) with ASP.NET MVC5. 

### IDK is a forum dedicated to programmers, where people can get help with different coding-related issues. 

Here is a list with some of the covered points, mostly examples of usage:
1. [Home page](#home)
2. [Asking a question](#ask)
3. [Answering a question](#answer)
4. [RBAC](#rbac)
5. [Used technologies and TODO](#more)
<a name="home"/>

## Home page
The default page contains a list of Tags and by clicking on Questions, the user is redirected to a page
containing only the questions which include that certain tag.
The navbar provides a searching tool which looks up the given phrase in question Titles, Content, or Comments

Home page:

![HomePage](https://github.com/cosminbvb/IDK/blob/master/screenshots/home.png?raw=true)

The section dedicated to a certain tag contains features such as:
- Pagination (3 posts on each page)
- Sorting by date or by the no. of answers

For example, the page dedicated to the posts with the C tag looks like this:

![C-Questions](https://github.com/cosminbvb/IDK/blob/master/screenshots/c-questions.png?raw=true)


<a name="ask"/>

## Asking a question

Every authenticated user that wants to post a question has to fill out this form:

![Ask](https://github.com/cosminbvb/IDK/blob/master/screenshots/ask.png?raw=true)

Each field is required and the user should choose the tags in order to describe the question as good as possible

<a name="answer"/>

## Answering a question

Authenticated users can comment on any post, the form for entering an answer being located below the last comment.
Here is an example:

![Ask](https://github.com/cosminbvb/IDK/blob/master/screenshots/question_answers.png?raw=true)


<a name="rbac"/>

## RBAC

Here is a brief description of the supported RBAC: 
- Unauthenticated user - only has the ability to view questions 
- Authenticated user - can ask/answer and has the ability to edit or permanently remove owned questions or comments.
- Editor - also has the power to edit or permanently remove any question / answer (e.g. when a question has a poor selection of tags, the editor can reassign the right tags)
- Admin - can also add new Tags and assign roles to users 

The Admin has a list of users and can change someone's role by clicking Edit

![Admin_Users](https://github.com/cosminbvb/IDK/blob/master/screenshots/admin_view_users.png?raw=true)

<a name="more"/>

## More

#### Used techologies:
- Entity Framework
- Bootstrap

#### TODO:
- Allow users to upvote questions/answers
- Elastic Search
- Develop a better feature for code formatting
- Create an eye candy interface 
