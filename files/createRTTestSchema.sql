/*
  This script creates and populates the schema needed to test
  the PL/SQL Regression Tester. It should be run under a user
  that has DBA privileges.
*/

-- @@@ Test something with lots of parameters.
-- @@@ Test something with CLOBs...


DROP USER rttests CASCADE;

CREATE USER rttests IDENTIFIED BY "JKSDFPO234Y6IUd7sf(*dfSD^%$" DEFAULT TABLESPACE users QUOTA UNLIMITED ON USERS;


CREATE TABLE rttests.customers (
  customer_id NUMBER(6),
  firstname   VARCHAR2(100),
  LASTNAME    VARCHAR2(100)
);

INSERT INTO rttests.customers
  (customer_id, firstname, lastname)
VALUES
  (1, 'John', 'Doe');
  
COMMIT;

/* Utilities package */

CREATE PACKAGE rttests.util AS 
  PROCEDURE assert(
    p_Expression   BOOLEAN,
    p_ErrorNumber  PLS_INTEGER := -20000,
    p_ErrorMessage VARCHAR2 := 'Assert failed.');
END UTIL;
/

CREATE PACKAGE BODY rttests.util AS
  PROCEDURE assert(
    p_Expression   BOOLEAN,
    p_ErrorNumber  PLS_INTEGER := -20000,
    p_ErrorMessage VARCHAR2 := 'Assert failed.') AS
  BEGIN
    IF p_ErrorNumber IS NULL THEN
      RAISE_APPLICATION_ERROR(-20912, 'The error number to raise cannot be NULL.');
    END IF;
  
    IF p_Expression IS NULL OR p_Expression = FALSE THEN
      RAISE_APPLICATION_ERROR(p_ErrorNumber, p_ErrorMessage);
    END IF;
  END;
END UTIL;
/

/* Definitions package */

CREATE OR REPLACE PACKAGE rttests.definitions AS
  SUBTYPE t_Firstname IS VARCHAR2(200);
  
  TYPE t_AddressRec IS RECORD(
    street  VARCHAR2(30),
    street2 VARCHAR2(30),
    city    VARCHAR2(30),
    state   VARCHAR2(30),
    zip     VARCHAR2(5)
  );
  
  TYPE t_CustomerRec IS RECORD (
    customer_id      NUMBER(6),
    firstname        t_Firstname,
    --middlename       VARCHAR2(20),
    lastname         VARCHAR2(20),
    comments         CLOB
  );
  TYPE t_arrCustomerRec IS TABLE OF t_CustomerRec;
  
  SUBTYPE t_CustRecRowType IS rttests.customers%ROWTYPE;
  TYPE t_arrCustRecs IS TABLE OF rttests.customers%ROWTYPE;
  
  TYPE t_arrVarchar2sIndexedByVarchr IS TABLE OF VARCHAR2(300) INDEX BY VARCHAR2(30);
  
  TYPE t_InvoiceRec IS RECORD (
    invoice_id    NUMBER(9),
    customer_id   NUMBER(6),
    creation_date DATE,
    some_flag     PLS_INTEGER,
    isRush        BOOLEAN,
    --isPastDue     BOOLEAN,
    total_value   NUMBER(10, 2)
  );
  TYPE t_arrInvoiceRecs IS TABLE OF t_InvoiceRec;
  
  TYPE t_arrPhoneNumbers IS TABLE OF VARCHAR2(30);
  
  TYPE t_AllCustInfoRec IS RECORD (
     customer_id   NUMBER(6),
     cust_info     t_CustomerRec,
     address_info  t_AddressRec,
     phone_numbers t_arrPhoneNumbers,
     arrInvoices   t_arrInvoiceRecs
  );
  TYPE t_arrAllCustInfoRecs IS TABLE OF t_AllCustInfoRec;
  
  TYPE t_CustomersRefCursor IS REF CURSOR RETURN t_CustomerRec;
END;
/

/* Methods for parameter change testing */

CREATE PACKAGE rttests.paramChanges AS
  FUNCTION gainingParameters(param1 VARCHAR2, param2 VARCHAR2 := 'ABC') RETURN VARCHAR2;
  
  FUNCTION losingParameters(param1 VARCHAR2 := 'abc') RETURN VARCHAR2;
  
  PROCEDURE renamingParameters(p_ID NUMBER, p_Name VARCHAR2);
END paramChanges;
/

CREATE OR REPLACE FUNCTION rttests.gainingParameters(param1 VARCHAR2, param2 VARCHAR2 := 'ABC') RETURN VARCHAR2 AS
BEGIN
  RETURN NULL;
END;
/
  
CREATE OR REPLACE FUNCTION rttests.losingParameters(param1 VARCHAR2 := 'abc') RETURN VARCHAR2 AS
BEGIN
  RETURN NULL;
END;
/

CREATE OR REPLACE FUNCTION rttests.losingNestedParameters(param1 definitions.t_AllCustInfoRec) RETURN VARCHAR2 AS
BEGIN
  RETURN NULL;
END;
/

/* Schema-level functions without parameters. */

CREATE OR REPLACE FUNCTION rttests.paramless_func_ret_pl_rowtype RETURN rttests.definitions.t_CustRecRowType AS
  v_Row rttests.customers%ROWTYPE;
BEGIN
  SELECT *
    INTO v_Row
    FROM rttests.customers
   WHERE rownum = 1;
   
  RETURN v_Row;
END;
/

CREATE FUNCTION rttests.paramless_func_ret_rowtype RETURN rttests.customers%ROWTYPE AS
  v_Row rttests.customers%ROWTYPE;
BEGIN
  SELECT *
    INTO v_Row
    FROM rttests.customers
   WHERE rownum = 1;
   
  RETURN v_Row;
END;
/

CREATE FUNCTION rttests.paramless_func_ret_varchar2 RETURN VARCHAR2 AS
BEGIN
  RETURN 'xyz';
END;
/

CREATE FUNCTION rttests.paramless_func_ret_null_vrchr RETURN VARCHAR2 AS
BEGIN
  RETURN NULL;
END;
/

CREATE FUNCTION rttests.paramless_func_ret_number RETURN NUMBER AS
BEGIN
  RETURN 123.456;
END;
/

CREATE FUNCTION rttests.paramless_func_ret_null_num RETURN NUMBER AS
BEGIN
  RETURN NULL;
END;
/

CREATE FUNCTION rttests.paramless_func_ret_date RETURN DATE AS
BEGIN
  RETURN TO_DATE('01/02/2003 04:05:06', 'MM/DD/YYYY HH24:MI:SS');
END;
/

CREATE FUNCTION rttests.paramless_func_null_date RETURN DATE AS
BEGIN
  RETURN NULL;
END;
/

CREATE FUNCTION rttests.paramless_func_raising_error RETURN VARCHAR2 AS
BEGIN
  RAISE_APPLICATION_ERROR(-20000, 'Something bad happened.');
  
  RETURN NULL;
END;
/

CREATE OR REPLACE FUNCTION rttests.table_varchar2_index_by_vrchr(param1 definitions.t_arrVarchar2sIndexedByVarchr) RETURN definitions.t_arrVarchar2sIndexedByVarchr AS
  retValue definitions.t_arrVarchar2sIndexedByVarchr;
BEGIN
  IF param1('a') != 'A' THEN
    RAISE_APPLICATION_ERROR(-20000, 'Index "a" is wrong.');
  END IF;
  
  IF param1('B') != 'B' THEN
    RAISE_APPLICATION_ERROR(-20001, 'Index "b" is wrong.');
  END IF;
  
  IF param1('c') != 'C' THEN
    RAISE_APPLICATION_ERROR(-20002, 'Index "c" is wrong.');
  END IF;
  
  RETURN retValue;
END;
/

/* Schema-level function with regular parameters */

CREATE FUNCTION rttests.param_func(p_Number NUMBER, p_Varchar2 VARCHAR2, p_Date DATE, p_PLS_Integer PLS_INTEGER, p_CLOB CLOB) RETURN VARCHAR2 AS
BEGIN
  RETURN p_Number || ',' || p_Varchar2 || ',' || TO_CHAR(p_Date, 'YYYYMMDD HH24:MI:SS') || ',' || p_PLS_Integer || ',' || p_CLOB;
END;
/

CREATE FUNCTION rttests.param_func_raising_error(p_Firstname VARCHAR2) RETURN VARCHAR2 AS
BEGIN
  RAISE_APPLICATION_ERROR(-20000, 'Something bad happened.');
  
  RETURN NULL;
END;
/

-- Schema-level result function returning array of records
CREATE OR REPLACE FUNCTION rttests.func_ret_row_array(
  p_CustomerID NUMBER,
  p_ObjCustomer rttests.customer,
  p_WeakRefCursor SYS_REFCURSOR, p_InOutWeakRefCursor IN OUT SYS_REFCURSOR, p_OutWeakRefCursor OUT SYS_REFCURSOR,
  p_StrongRefCursor definitions.t_CustomersRefCursor, p_InOutStrongRefCursor IN OUT definitions.t_CustomersRefCursor, p_OutStrongRefCursor OUT definitions.t_CustomersRefCursor, 
  p_arrCustRecs definitions.t_arrCustRecs, 
  p_arrAllCustInfoRecs definitions.t_arrAllCustInfoRecs, 
  p_LastParam CLOB, 
  p_inoutArrCustInfoRecs IN OUT definitions.t_arrAllCustInfoRecs, 
  p_OutArrCustInfoRecs OUT definitions.t_arrAllCustInfoRecs
) RETURN definitions.t_arrAllCustInfoRecs IS
  v_AllCustInfoRec     definitions.t_AllCustInfoRec;
  
  v_arrAllCustInfoRecs definitions.t_arrAllCustInfoRecs := definitions.t_arrAllCustInfoRecs();
BEGIN
  OPEN p_OutWeakRefCursor FOR
    SELECT 'John' AS firstname, 'Doe' AS lastname FROM dual
    UNION ALL
    SELECT 'Mickey' AS firstname, 'Mouse' AS lastname FROM dual;

  v_arrAllCustInfoRecs.EXTEND(2);

  v_AllCustInfoRec.cust_info.customer_id := 1;
  v_AllCustInfoRec.cust_info.firstname := 'Joe';
  v_AllCustInfoRec.cust_info.lastname := 'Smoe';
  
  v_AllCustInfoRec.address_info.street := '123 Acme Lane';
  v_AllCustInfoRec.address_info.street2 := 'Apt. 456';
  v_AllCustInfoRec.address_info.city := 'Nowhere';
  v_AllCustInfoRec.address_info.state := 'AR';
  v_AllCustInfoRec.address_info.zip := '12345';
  
  v_arrAllCustInfoRecs(1) := v_AllCustInfoRec;
    
  v_AllCustInfoRec.cust_info.customer_id := 2;
  v_AllCustInfoRec.cust_info.firstname := 'John';
  v_AllCustInfoRec.cust_info.lastname := 'Doe';
  
  v_AllCustInfoRec.address_info.street := '53 Horseshoes Lane';
  v_AllCustInfoRec.address_info.street2 := 'Apt. A';
  v_AllCustInfoRec.address_info.city := 'Somewhere';
  v_AllCustInfoRec.address_info.state := 'NY';
  v_AllCustInfoRec.address_info.zip := '78645';
  
  v_arrAllCustInfoRecs(2) := v_AllCustInfoRec;

  RETURN v_arrAllCustInfoRecs;
END;
/

-- Schema-level function with IN, IN/OUT, and OUT parameters of various types

CREATE FUNCTION rttests.param_func_in_out(
  p_InNumber    IN     NUMBER, p_InVarchar2    IN     VARCHAR2, p_InDate    IN     DATE, p_InPLS_Integer    IN     PLS_INTEGER, p_InCLOB    IN     CLOB, p_InFirstname    IN     definitions.t_Firstname, p_InSysRefCursor    IN     SYS_REFCURSOR,
  p_InOutNumber IN OUT NUMBER, p_InOutVarchar2 IN OUT VARCHAR2, p_InOutDate IN OUT DATE, p_InOutPLS_Integer IN OUT PLS_INTEGER, p_InOutCLOB IN OUT CLOB, p_InOutFirstname IN OUT definitions.t_Firstname, p_InOutSysRefCursor IN OUT SYS_REFCURSOR,
  p_OutNumber      OUT NUMBER, p_OutVarchar2      OUT VARCHAR2, p_OutDate      OUT DATE, p_OutPLS_Integer      OUT PLS_INTEGER, p_OutCLOB      OUT CLOB, p_OutFirstname      OUT definitions.t_Firstname, p_OutSysRefCursor      OUT SYS_REFCURSOR
  ) RETURN SYS_REFCURSOR
AS
  v_InTempVarchar2    VARCHAR2(32767);
  v_InOutTempVarchar2 VARCHAR2(32767);
  
  v_ReturnValue SYS_REFCURSOR;
BEGIN
  FETCH p_InSysRefCursor INTO v_InTempVarchar2;
  CLOSE p_InSysRefCursor;

  p_InOutNumber      := p_InNumber * p_InOutNumber;
  p_InOutVarchar2    := p_InVarchar2 || ',' || p_InOutVarchar2;
  p_InOutDate        := TO_DATE('01/01/2000', 'MM/DD/YYYY') + (TRUNC(p_InDate) - TRUNC(p_InOutDate));
  p_InOutPLS_Integer := p_InPLS_Integer * p_InOutPLS_Integer;
  p_InOutCLOB        := p_InCLOB || ',' || p_InOutCLOB;
  p_InOutFirstname   := p_InFirstname || ',' || p_InOutFirstname;
    
  FETCH p_InOutSysRefCursor INTO v_InOutTempVarchar2;
  CLOSE p_InOutSysRefCursor;
  
  OPEN p_InOutSysRefCursor FOR
    SELECT '*' || v_InTempVarchar2 || '*' AS col1, '*' || v_InOutTempVarchar2 || '*' AS col2
      FROM dual;
  
  p_OutNumber      := p_InNumber * 2;
  p_OutVarchar2    := '*' || p_InVarchar2 || '*';
  p_OutDate        := p_InDate + 3;
  p_OutPLS_Integer := p_InPLS_Integer * 5;
  p_OutCLOB        := '|' || p_InCLOB || '|';
  p_OutFirstname   := 'Mickey';
  
  OPEN p_OutSysRefCursor FOR
    SELECT 1 AS customer_id, 'John' AS firstname, 'Doe' AS lastname FROM dual
    UNION ALL
    SELECT 2 AS customer_id, 'Jane' AS firstname, 'Doe' AS lastname FROM dual;

  OPEN v_ReturnValue FOR
    SELECT 50 AS customer_id, 'Mickey' AS firstname, 'Johnson' AS lastname FROM dual
    UNION ALL
    SELECT 51 AS customer_id, 'Curly' AS firstname, 'Moe' AS lastname FROM dual;

  RETURN v_ReturnValue;
END;
/

CREATE OR REPLACE FUNCTION rttests.param_func_rowtype(p_CustRec definitions.t_CustRecRowType) RETURN VARCHAR2 AS
BEGIN
  RETURN p_CustRec.firstname || ' ' || p_CustRec.lastname;
END;
/

-- @@@ Schema-level pipelined result function returning scalar array


-- Schema-level pipelined result function returning row type
CREATE FUNCTION rttests.pipelined_result_func_ret_row RETURN definitions.t_arrCustomerRec PIPELINED IS
  v_CustRow definitions.t_CustomerRec;
BEGIN
  v_CustRow.customer_id := 1;
  v_CustRow.firstname := 'Joe';
  v_CustRow.lastname := 'Smoe';
  PIPE ROW (v_CustRow);
    
  v_CustRow.customer_id := 2;
  v_CustRow.firstname := 'John';
  v_CustRow.lastname := 'Doe';
  PIPE ROW (v_CustRow);

  RETURN;
END;
/

-- Schema-level pipelined result function raising an error
CREATE FUNCTION rttests.pipelined_result_func_w_error RETURN definitions.t_arrCustomerRec PIPELINED IS
  v_CustRow definitions.t_CustomerRec;
BEGIN
  v_CustRow.customer_id := 1;
  v_CustRow.firstname := 'Joe';
  v_CustRow.lastname := 'Smoe';
  PIPE ROW (v_CustRow);
    
  v_CustRow.customer_id := 2;
  v_CustRow.firstname := 'John';
  v_CustRow.lastname := 'Doe';
  PIPE ROW (v_CustRow);

  RAISE_APPLICATION_ERROR(-20000, 'Something bad happened.');

  RETURN;
END;
/

CREATE OR REPLACE FUNCTION rttests.raw_param_func(p_MyRaw RAW) RETURN RAW AS
BEGIN
  RETURN '*' || p_MyRaw || '*';
END;
/

/* Ref Cursors */

CREATE OR REPLACE FUNCTION rttests.rfcur_strong_param_func(p_Case PLS_INTEGER) RETURN definitions.t_CustomersRefCursor AS
  v_CustomersRefCursor definitions.t_CustomersRefCursor;
BEGIN
  CASE p_Case
  WHEN 1 THEN
    OPEN v_CustomersRefCursor FOR
      SELECT 50 AS customer_id, 'Mickey' AS firstname, 'Johnson' AS lastname, 'Some comment' AS comments FROM dual
      UNION ALL
      SELECT 51 AS customer_id, 'Curly' AS firstname, 'Moe' AS lastname, 'Other comments' AS comments FROM dual;
      
  WHEN 2 THEN
    -- Return NULL ref cursor
    NULL;
    
  WHEN 3 THEN
    -- Return empty ref cursor
    OPEN v_CustomersRefCursor FOR
      SELECT 50 AS customer_id, 'Mickey' AS firstname, 'Johnson' AS lastname, 'Some comment' AS comments FROM dual
       WHERE 1 = 0;    
  END CASE;
    
  RETURN v_CustomersRefCursor;
END;
/

CREATE OR REPLACE FUNCTION rttests.rfcur_strong_var_inout(
  p_Case  IN     PLS_INTEGER,
  p_In    IN     definitions.t_CustomersRefCursor,
  p_InOut IN OUT definitions.t_CustomersRefCursor,
  p_Out      OUT definitions.t_CustomersRefCursor)
RETURN rttests.definitions.t_CustomersRefCursor AS
  v_RetVal definitions.t_CustomersRefCursor;
  
  v_In    definitions.t_CustomerRec;
  v_InOut definitions.t_CustomerRec;
  v_Out   definitions.t_CustomerRec;
BEGIN
  CASE p_Case
  WHEN 1 THEN
    FETCH p_In INTO v_In;
    CLOSE p_In;
    
    IF v_In.customer_id != 48 THEN
      RAISE_APPLICATION_ERROR(-20000, 'Expected a customer ID of 48.');
    END IF;
    
    FETCH p_InOut INTO v_InOut;    
    CLOSE p_InOut;
    
    IF v_InOut.customer_id != 49 THEN
      RAISE_APPLICATION_ERROR(-20001, 'Expected a customer ID of 49.');      
    END IF;
    
    OPEN p_InOut FOR
      SELECT 52 AS customer_id, 'Minnie' AS firstname, 'Mouse' AS lastname, 'Rodentia' AS comments FROM dual;
      
    OPEN p_Out FOR
      SELECT 53 AS customer_id, 'Hueie' AS firstname, 'Duck' AS lastname, 'Duck tales' AS comments FROM dual;
      
    OPEN v_RetVal FOR
      SELECT 54 AS customer_id, 'Donald' AS firstname, 'Duck' AS lastname, 'Livingston' AS comments FROM dual;
        
  WHEN 2 THEN
    CLOSE p_InOut;
    p_InOut := NULL;
    
    p_Out := NULL;
  
  WHEN 3 THEN    
    OPEN p_InOut FOR
      SELECT 1 AS customer_id, '2' AS firstname, '3' AS lastname, '4' AS comments FROM dual
      UNION ALL
      SELECT 5 AS customer_id, '6' AS firstname, '7' AS lastname, '8' AS comments FROM dual;
      
    OPEN p_Out FOR
      SELECT 9 AS customer_id, '10' AS firstname, '11' AS lastname, '12' AS comments FROM dual
      UNION ALL
      SELECT 13 AS customer_id, '14' AS firstname, '15' AS lastname, '16' AS comments FROM dual;
      
    OPEN v_RetVal FOR
      SELECT 17 AS customer_id, '18' AS firstname, '19' AS lastname, '20' AS comments FROM dual
      UNION ALL
      SELECT 21 AS customer_id, '22' AS firstname, '23' AS lastname, '24' AS comments FROM dual;

  END CASE;
  
  RETURN v_RetVal;
END;
/

CREATE FUNCTION rttests.rfcur_weak_paramless_func RETURN SYS_REFCURSOR AS
  v_RetValue SYS_REFCURSOR;
BEGIN
  OPEN v_RetValue FOR
    SELECT 50 AS customer_id, 'Mickey' AS firstname, 'Johnson' AS lastname FROM dual
    UNION ALL
    SELECT 51 AS customer_id, 'Curly' AS firstname, 'Moe' AS lastname FROM dual;
    
  RETURN v_RetValue;
END;
/

CREATE OR REPLACE FUNCTION rttests.rfcur_weak_param_func(p_Case PLS_INTEGER) RETURN SYS_REFCURSOR AS
  v_RetValue SYS_REFCURSOR;
BEGIN
  CASE p_Case
  WHEN 1 THEN
    -- Return a cursor with something in it...
    OPEN v_RetValue FOR
      SELECT 50 AS customer_id, 'Mickey' AS firstname, 'Johnson' AS lastname FROM dual
      UNION ALL
      SELECT 51 AS customer_id, 'Curly' AS firstname, 'Moe' AS lastname FROM dual;
  WHEN 2 THEN
    -- Return null
    NULL;
  WHEN 3 THEN
    -- No rows
    OPEN v_RetValue FOR
      SELECT 1
        FROM dual
      WHERE dummy = '1';
  END CASE;
    
  RETURN v_RetValue;
END;
/

CREATE OR REPLACE FUNCTION rttests.rfcur_weak_var_inout(
  p_Case  IN PLS_INTEGER,
  p_In    IN SYS_REFCURSOR,
  p_InOut IN OUT SYS_REFCURSOR,
  p_Out      OUT SYS_REFCURSOR)
RETURN SYS_REFCURSOR AS
  v_RetVal SYS_REFCURSOR;
  
  v_InNum    PLS_INTEGER;
  v_InOut    PLS_INTEGER;
  v_Out      PLS_INTEGER;
BEGIN
  CASE p_Case
  WHEN 1 THEN
    FETCH p_In INTO v_InNum;
    FETCH p_InOut INTO v_InOut;
    
    CLOSE p_InOut;
    OPEN p_InOut FOR
      SELECT v_InNum * v_InOut FROM dual;
      
    OPEN p_Out FOR
      SELECT v_InNum + v_InOut FROM dual;
      
    OPEN v_RetVal FOR
      SELECT v_InNum * v_InOut * 2 FROM dual;
        
  WHEN 2 THEN
    CLOSE p_InOut;
    p_InOut := NULL;
    
    p_Out := NULL;
  END CASE;
  
  RETURN v_RetVal;
END;
/



-- Schema-level procedure without parameters
-- Schema-level procedure with regular parameters
-- Schema-level procedure with IN, IN/OUT, and OUT parameters of various types
-- Schema-level procedure with a SYSREFCURSOR parameter
-- Schema-level procedure with a SUBTYPE parameter
-- Schema-level procedure raising an error.
-- Schema-level procedure with SYSREFCURSOR parameters and a SYSREFCURSOR return type

-- Package-level function without parameters
-- Package-level function with regular parameters
-- Package-level function with IN, IN/OUT, and OUT parameters of various types
-- Package-level function with a SYSREFCURSOR parameter
-- Package-level function with a SUBTYPE parameter
-- Package-level function with SYSREFCURSOR parameters and a SYSREFCURSOR return type
-- Package-level pipelined result function returning scalar array
-- Package-level pipelined result function returning record row type

-- Package-level procedure without parameters
-- Package-level procedure with regular parameters
-- Package-level procedure with IN, IN/OUT, and OUT parameters of various types
-- Package-level procedure with a SYSREFCURSOR parameter
-- Package-level procedure with a SUBTYPE parameter
-- Package-level procedure with SYSREFCURSOR parameters and a SYSREFCURSOR return type

-- Table with a trigger on it

-- View with an instead-of trigger

-- Object
CREATE TYPE rttests.customer AS OBJECT (
   customer_id NUMBER(6),
   firstname   VARCHAR2(30),
   lastname    VARCHAR2(30)
);