SELECT "Id", "Title", "Description", "Status", "CreatedAt", "UpdatedAt"
	FROM "TechnicalAPI"."Tasks";

ALTER TABLE "TechnicalAPI"."Tasks"
	ALTER COLUMN "Status" TYPE "TechnicalAPI".enum_type 
		using "Status"::"TechnicalAPI".enum_type;

CREATE TYPE "TechnicalAPI".enum_type AS ENUM ('ToDo', 'InProgress', 'Done');


INSERT INTO "TechnicalAPI"."Tasks" ("Id", "Title", "Description", "Status", "CreatedAt", "UpdatedAt")
VALUES ('3fa85f64-5717-4562-b3fc-2c963f66afa6', 'TaskOne', 'Test Task', 'ToDo', '2025-05-13 00:00:00.000', '2025-05-13 00:00:00.000')


SELECT column_name, data_type
FROM information_schema.columns
WHERE table_name = 'Tasks' AND column_name = 'Status';


ALTER TABLE "TechnicalAPI"."Tasks" 
ALTER COLUMN "UpdatedAt" TYPE TIMESTAMP;

ALTER TABLE "TechnicalAPI"."Tasks" 
ALTER COLUMN "CreatedAt" TYPE TIMESTAMP;


TRUNCATE TABLE "TechnicalAPI"."Tasks"



CREATE TABLE "TechnicalAPI"."Tokens" (
    "Id" UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    "Token" TEXT NOT NULL,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "ExpiresAt" TIMESTAMP NOT NULL
);