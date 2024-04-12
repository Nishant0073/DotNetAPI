
Use DotNetCourseDatabase;
Go

ALTER PROCEDURE TutorialAppSchema.spUser_Get
-- EXEC TutorialAppSchema.spUser_Get
AS
BEGIN
    SELECT [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]  FROM TutorialAppSchema.Users AS Users;
END
GO

-- ALTER PROCEDURE TutorialAppSchema.spUser_Get_By_Id
-- -- EXEC TutorialAppSchema.spUser_Get_By_Id @UserId = 3
--     @UserId INT = NULL
-- AS
-- BEGIN
--     SELECT [Users].[UserId],
--     [Users].[FirstName],
--     [Users].[LastName],
--     [Users].[Email],
--     [Users].[Gender],
--     [Users].[Active] FROM TutorialAppSchema.Users AS Users 
--     WHERE [Users].UserId = ISNULL(@UserId,Users.UserId)
-- END
-- GO


ALTER PROCEDURE TutorialAppSchema.spUser_Get_By_Id
-- EXEC TutorialAppSchema.spUser_Get_By_Id @UserId = 3
    @UserId INT = NULL
AS
BEGIN
    -- SELECT UserJobInfo.Department
    -- , AVG(UserSalary.Salary) AvgSalary
    --  FROM TutorialAppSchema.Users AS Users 
    --     LEFT JOIN TutorialAppSchema.UserSalary As UserSalary
    --         on Users.UserId = UserSalary.UserId
    --     LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
    --         on Users.UserId = UserSalary.UserId
    --     GROUP BY Department;    
    SELECT [Users].[UserId],
    [Users].[FirstName],
    [Users].[LastName],
    [Users].[Email],
    [Users].[Gender],
    [Users].[Active],
    UserSalary.Salary,
    UserJobInfo.JobTitle
    FROM TutorialAppSchema.Users AS Users 
        LEFT JOIN TutorialAppSchema.UserSalary As UserSalary
            on Users.UserId = UserSalary.UserId
        LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
            on Users.UserId = UserSalary.UserId
        OUTER APPLY(
        SELECT UserJobInfo.Department
        , AVG(UserSalary.Salary) AvgSalary
        FROM TutorialAppSchema.Users AS Users 
            LEFT JOIN TutorialAppSchema.UserSalary As UserSalary
                on Users.UserId = UserSalary.UserId
            LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
                on Users.UserId = UserSalary.UserId
            GROUP BY Department
        ) AS AvgSalary
    WHERE [Users].UserId = ISNULL(@UserId,Users.UserId)
END
GO


