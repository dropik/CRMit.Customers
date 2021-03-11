# CRMit.Customers
Microservice responsible for Customer resource in CRMit API v1.

## Getting Started
Currently testable in a single host environment via docker-compose. To start microservice, run  
```cmd
> run.cmd
```

# API Resource: Customers

## Customer

Represents a customer which this CRM system targets.

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| id | long | Customer's unique number.<br>_Example:_ `123456` | Yes |
| name | string | Name.<br>_Example:_ `"Ivan"` | Yes |
| surname | string | Surname.<br>_Example:_ `"Petrov"` | No |
| email | string (email) | Email address.<br>_Example:_ `"ivan.petrov@example.com"` | Yes |

## Operations

### /crmit/v1/Customers

#### POST
##### Summary

Create a new customer.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| body | body | Customer data to be used for new customer. | Yes | [CustomerInput](#customerinput) |

##### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 201 | Customer created and standard Created response returned with new customer. | [Customer](#customer) |
| 400 | Invalid parameters passed. | [ProblemDetails](#problemdetails) |
| 500 | Internal error occured. |  |

#### GET
##### Summary

Get a list containing all customers.

##### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | List with all customers. | [ [Customer](#customer) ] |
| 500 | Internal error occured. |  |

### /crmit/v1/Customers/{id}

#### GET
##### Summary

Get a customer by id.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Customer id. | Yes | long |

##### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Found customer. | [Customer](#customer) |
| 400 | Invalid parameters passed. | [ProblemDetails](#problemdetails) |
| 404 | Customer not found. | [ProblemDetails](#problemdetails) |
| 500 | Internal error occured. |  |

#### PUT
##### Summary

Update a customer given id.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Customer id. | Yes | long |
| body | body | Customer data to update with. | Yes | [CustomerInput](#customerinput) |

##### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Customer updated. |  |
| 400 | Invalid parameters passed. | [ProblemDetails](#problemdetails) |
| 404 | Customer not found. | [ProblemDetails](#problemdetails) |
| 500 | Internal error occured. |  |

#### DELETE
##### Summary

Delete a customer given id.

##### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ---- |
| id | path | Customer id. | Yes | long |

##### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | Customer deleted. |  |
| 400 | Invalid parameters passed. | [ProblemDetails](#problemdetails) |
| 404 | Customer not found. | [ProblemDetails](#problemdetails) |
| 500 | Internal error occured. |  |

### Models

#### CustomerInput

Customer input data.

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| name | string | Name.<br>_Example:_ `"Ivan"` | Yes |
| surname | string | Surname.<br>_Example:_ `"Petrov"` | No |
| email | string (email) | Email address.<br>_Example:_ `"ivan.petrov@example.com"` | Yes |

#### ProblemDetails

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| type | string |  | No |
| title | string |  | No |
| status | integer |  | No |
| detail | string |  | No |
| instance | string |  | No |

## Changelogs
### v1.0.0-preview
* Added basic CRUD operations on Customers resource.
