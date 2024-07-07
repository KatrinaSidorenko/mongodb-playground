# MongoDB Playground for .NET

This console project serves as a playground for testing various features of the MongoDB driver for .NET. It includes sample models, schema validation, and indexes for users and products.

## Table of Contents

- [Introduction](#introduction)
- [Models Schema](#models-schema)
  - [User Schema](#user-schema)
  - [Product Schema](#product-schema)
- [Schema Validation](#schema-validation)
  - [User Schema Validation](#user-schema-validation)
  - [Product Schema Validation](#product-schema-validation)
- [Indexes](#indexes)
  - [User Indexes](#user-indexes)
  - [Product Indexes](#product-indexes)
- [Usage](#usage)
  - [Installation](#installation)
  - [Running the Project](#running-the-project)
- [Features](#features)

## Introduction

This project provides a structured way to interact with MongoDB using the MongoDB driver for .NET. It includes example schemas for `User` and `Product`, along with schema validation and indexes to ensure data integrity and efficient querying.

## Models Schema

### User Schema

```json
{
  "_id": ObjectId,
  "username": String,
  "email": String,
  "passwordHash": String
}
```

### Product Schema

```json
{
  "_id": ObjectId,
  "name": String,
  "description": String,
  "price": Number,
  "stock": Number,
  "createdAt": Date,
  "reviews": [
    {
      "userId": ObjectId,
      "username": String,
      "rating": Number,
      "comment": String,
      "createdAt": Date
    }
  ]
}
```

## Schema Validation

Schema validation is enforced in MongoDB to ensure that documents conform to a specified structure.

### User Schema Validation

```javascript
db.createCollection("users", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            title: "User Object Validation",
            required: ["username", "email", "passwordHash"],
            properties: {
                username: {
                    bsonType: "string",
                    description: "'username' must be a string and is required"
                },
                email: {
                    bsonType: "string",
                    pattern: "^.+@.+\..+$",
                    description: "must be a string and match the email pattern"
                },
                passwordHash: {
                    bsonType: "string",
                    description: "must be a string and is required"
                }
            }
        }
    }
});
```

### Product Schema Validation

```javascript
db.createCollection("products", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["name", "price", "stock", "createdAt"],
            properties: {
                name: {
                    bsonType: "string",
                    description: "must be a string and is required"
                },
                description: {
                    bsonType: "string",
                    description: "must be a string and is optional"
                },
                price: {
                    bsonType: "number",
                    description: "must be a number and is required"
                },
                stock: {
                    bsonType: "number",
                    description: "must be a number and is required"
                },
                createdAt: {
                    bsonType: "date",
                    description: "must be a date and is required"
                },
                reviews: {
                    bsonType: "array",
                    items: {
                        bsonType: "object",
                        required: ["userId", "rating"],
                        properties: {
                            userId: {
                                bsonType: "objectId",
                                description: "must be an objectId and is required"
                            },
                            username: {
                                bsonType: "string",
                                description: "must be a string and is required"
                            },
                            rating: {
                                bsonType: "number",
                                minimum: 1,
                                maximum: 5,
                                description: "must be a number between 1 and 5 and is required"
                            },
                            comment: {
                                bsonType: "string",
                                description: "must be a string and is optional"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "must be a date and is required"
                            }
                        }
                    }
                }
            }
        }
    }
});
```

## Indexes

### User Indexes

Create a unique index on the `email` field:

```javascript
db.users.createIndex({ "email": 1 }, { unique: true });
```

Create a compound index on the `email` and `username` fields:

```javascript
db.users.createIndex({ "email": 1, "username": 1 }, { unique: true });
```

### Product Indexes

No specific indexes for the product schema are defined in the example.

## Usage

### Installation

1. Clone the repository:
   ```sh
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```sh
   cd <project-directory>
   ```
3. Install the required packages:
   ```sh
   dotnet restore
   ```

### Running the Project

To run the project, use the following command:
```sh
dotnet run
```

## Features

- Create, read, update, and delete operations for users and products.
- Schema validation for ensuring data integrity.
- Example usage of MongoDB driver for .NET with sessions.

Feel free to explore and modify the project to test different features of the MongoDB driver for .NET!