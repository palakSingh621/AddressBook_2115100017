# AddressBook_2115100017
**UC 1: Configure Database and Application Settings**
 - Define Database Connection in appsettings.json
 - Set up Entity Framework Core Migrations
 - Implement DbContext for database interaction
 - Configure Dependency Injection (DI) for database services

**UC 2: Implement Address Book API Controller**
 - Define RESTful Endpoints: 
   - GET /api/addressbook → Fetch all contacts
   - GET /api/addressbook/{id} → Get contact by ID
   - POST /api/addressbook → Add a new contact
   - PUT /api/addressbook/{id} → Update contact
   - DELETE /api/addressbook/{id} → Delete contact
 - Use ActionResult<T> to return JSON responses
 - Test using Postman or CURL

**UC 4: Implement Address Book Service Layer and API Documentation**
 - Create IAddressBookService interface
 - Implement AddressBookService: 
 - Move logic from controller to service layer
 - Handle CRUD operations in business logic
 - Inject Service Layer into Controller using Dependency Injection
 -  Document API with Swagger
 - Enable Swagger UI for API testing
 - Define request/response models in Swagger
 - Auto-generate API documentation

**UC 5: Implement User Registration & Login**
 - Create User Model & DTO
 - Implement Password Hashing (BCrypt)
 - Generate JWT Token on successful login
 - Store User Data in MS SQL Database Endpoints: 
  - POST /api/auth/register
  - POST /api/auth/login

**UC 6: Implement Forgot & Reset Password**
 - Generate Reset Token
 - Send Password Reset Email (SMTP)
 - Verify token & allow password reset Endpoints: 
  - POST /api/auth/forgot-password
  - POST /api/auth/reset-password

**UC 7: Integrate Redis for Caching**
 - Store  Session Data in Redis
 - Cache Address Book Data for faster access
 - Improve performance & reduce DB calls

**UC 8: Integrate RabbitMQ for Event-Driven Messaging**
 - Publish events when: 
 - New user registers (Send email)
 - Contact is added to Address Book
 - Consume messages asynchronously

**UC 9: Testing APIs Using NUnit**
 - Test User Authentication
 - Test CRUD operations for Address Book
 - Validate Email Sending, JWT, and Redis
