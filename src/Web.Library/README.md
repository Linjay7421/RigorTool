## Where to put interface?

### Conditions:

- Handler or Service need to use: put in Application.
- Not required outside: keep in Infrastructure.

```
Application
        │
        ▼
+-----------------------+
| IProductRepository    |
| IStorageProvider      |
| IEmailSender          |
| IClock               |
+-----------------------+
        ▲
        │
Infrastructure
        │
+-----------------------+
| ProductRepository     |
| LocalStorageProvider  |
| SmtpEmailSender       |
| SystemClock           |
+-----------------------+
```