 -- Member테이블
CREATE TABLE Member 
(
    ID VARCHAR(50) PRIMARY KEY,
    PW VARCHAR(50) NOT NULL,
    NAME VARCHAR(50) NOT NULL
);

 -- Member_Info테이블
CREATE TABLE Member_Info
(
    ID VARCHAR(50) NOT NULL,
    Q VARCHAR(MAX) NOT NULL,
    A VARCHAR(MAX) NOT NULL,
    CODE VARCHAR(MAX) NOT NULL,
    FOREIGN KEY(ID) REFERENCES Member(ID)
);

 -- 저장 프로시저
CREATE PROCEDURE SAVECODE
(
   @ID varchar(50),
   @QUESTION varchar(MAX),
   @ANSWER varchar(MAX),
   @CODE varchar(MAX)
)
AS
   IF Exists (Select ID from MEMBER_INFO where ID = @ID )
   BEGIN
      UPDATE Member_info SET Q = @QUESTION, A = @ANSWER, CODE = @CODE WHERE ID = @ID;
   END
   
   ELSE
   BEGIN
      INSERT INTO Member_info VALUES(@ID, @QUESTION, @ANSWER, @CODE);
   END
   
RETURN