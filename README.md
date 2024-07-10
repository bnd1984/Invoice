# Invoice System

This is a simple invoice system API built with ASP.NET Core. It allows creating invoices, paying invoices, and processing overdue invoices.

## Endpoints

- `POST /api/invoices`: Create a new invoice.
- `GET /api/invoices`: Get all invoices.
- `POST /api/invoices/{id}/payments`: Pay an invoice.
- `POST /api/invoices/process-overdue`: Process overdue invoices.

## Assumptions

- Invoices are stored in a JSON file (`invoices.json`).
- The system processes overdue invoices based on a given late fee and overdue days.

## Running the Project

1. Clone the repository.
2. Navigate to the project directory.
3. Run `dotnet run`.



