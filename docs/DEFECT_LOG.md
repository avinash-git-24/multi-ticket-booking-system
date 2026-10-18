# Defect Log and Resolution Register

| ID | Phase | Module | Symptom | Root Cause | Fix Applied | Verification Method | Remaining Risk | Status |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **DEF-001** | Phase 2 | Database Indexes | `Msg 1934: CREATE INDEX failed because QUOTED_IDENTIFIER is OFF` during `02_indexes.sql` execution | SQL Server requires `ANSI_NULLS` and `QUOTED_IDENTIFIER` set to `ON` when defining filtered indexes | Added `SET ANSI_NULLS ON;` and `SET QUOTED_IDENTIFIER ON;` in `database/02_indexes.sql` | Executed via `sqlcmd -S "(localdb)\MSSQLLocalDB"`: completed with return code 0 and zero errors | None | Closed |
