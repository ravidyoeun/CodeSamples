## Welcome to Quotemule - Code Samples

Quotemule is an online portal that connects suppliers providing various goods and services to buyers in the construction industry from across USA. Government agencies in USA spend over $100 billion annually on various products and services. With Quotemule, suppliers can get a share of these contracts. Suppliers interested in doing business with public and private purchasing agencies create unique company profiles on Quotemule, and define their criteria. Quotemule, searches its database for local contracts and sends an SMS alert when bids have been accepted.

[http://quotemule.azurewebsites.net/]

### State Machine Wizard

The state machine wizard was implemented because we wanted to keep track of the current stages of the quote request workflow made by users. Users must carry out a series of actions, or handle inputs depending on what the state it's in. The state machine UI was created using HTML/CSS/JavaScript and AngularJS. In order to switch between views efficiently without reloading, NG-ROUTE was used to handle the transitioning of views. For the middle-tier, I implemented an Api Controller in C# which updated the current state that is handled by Stateless .NET library in the services layer. After a stage is completed, Quotemule would send a SMS text to the user letting them know whether the contact was awarded through Twilio API. 

### User Invitation

Companies could invite users who were not already registered in the database by typing in the user's email and asign a role. A dynamic email populated with the company's information would then be sent to the invited user directing them to sign up with a url. I used SendGrid API to allow email to be sent via HTTP and RazorEngine to parse the email's HTML template with dynamic values. 

### User Profile

The User profiles on Quotemule are unique because they help identify the roles of the users. I implemented DropZone on the profile pages to allow users to upload pictures connected to our Amazon s3 services. 


