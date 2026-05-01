-- ============================================================
-- AUTOTEK DATABASE - FULL SCHEMA SCRIPT
-- PostgreSQL 16.2
-- Dumped: 2025-04-29 | Reorganized for clean recreation
-- Run this entire script on a fresh database to recreate schema
-- ============================================================

-- ============================================================
-- SECTION 1: SEQUENCES
-- ============================================================

CREATE SEQUENCE public.assigncodeseq
    START WITH 2000000
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE public.item_details_detail_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE SEQUENCE public.transactions_transaction_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


-- ============================================================
-- SECTION 2: CREATE TABLE SCRIPTS (with all columns)
-- ============================================================

-- ------------------------------------------------------------
-- TABLE: registeragent
-- Stores user accounts (login credentials, plan info)
-- ------------------------------------------------------------
CREATE TABLE public.registeragent (
    phonenumber             numeric,
    _password               character varying(255)  NOT NULL,
    _state                  character varying(255)  NOT NULL,
    address                 character varying(255)  NOT NULL,
    status                  character varying(255)  DEFAULT 'PARTIAL'::character varying,
    expirydate              date                    DEFAULT (CURRENT_DATE + 1),
    plantype                character varying(10)
);


-- ------------------------------------------------------------
-- TABLE: addbusinessinformation
-- Stores business profile for each registered user
-- ------------------------------------------------------------
CREATE TABLE public.addbusinessinformation (
    registeredphonenumber   numeric                 NOT NULL,
    businessname            character varying(50),
    gstin                   character varying(50),
    phonenumber             numeric,
    emailid                 character varying(255),
    businessaddress         character varying(300),
    businesstype            character varying(100),
    businesscategory        character varying(100),
    pincode                 numeric,
    state                   character varying(100),
    businessdescription     character varying(300)
);


-- ------------------------------------------------------------
-- TABLE: category
-- Product categories (scoped per registered user)
-- ------------------------------------------------------------
CREATE TABLE public.category (
    category                character varying(100),
    registeredphonenumber   numeric
);


-- ------------------------------------------------------------
-- TABLE: partygroup
-- Customer/vendor groups (scoped per registered user)
-- ------------------------------------------------------------
CREATE TABLE public.partygroup (
    partygroup              character varying(255),
    registeredphonenumber   numeric
);


-- ------------------------------------------------------------
-- TABLE: party
-- Customers and vendors with balance tracking
-- ------------------------------------------------------------
CREATE TABLE public.party (
    partyname               character varying(255),
    gst                     character varying(255)  DEFAULT ''::character varying,
    phonenumber             numeric,
    partygroup              character varying(255),
    gsttype                 character varying(255),
    _state                  character varying(255),
    emailid                 character varying(255),
    billingaddress          character varying(255),
    shippingaddress         character varying(255),
    openingbalance          numeric                 DEFAULT 0,
    asofdate                date,
    creditlimit             numeric                 DEFAULT 0,
    additionalfieldname1    character varying(255),
    additionalfieldname2    character varying(255),
    additionalfieldname3    character varying(255),
    additionalfieldname4    character varying(255),
    registeredphonenumber   numeric,
    topayorreceive          character varying,
    typeofpay               character varying,
    partybalance            numeric                 DEFAULT 0,
    additionalfieldname1value character varying(255),
    additionalfieldname2value character varying(255),
    additionalfieldname3value character varying(255),
    topayparty              numeric                 DEFAULT 0,
    toreceivefromparty      numeric                 DEFAULT 0,
    additionalfieldname4value character varying(20) DEFAULT ''::character varying
);


-- ------------------------------------------------------------
-- TABLE: item
-- Inventory items with pricing, tax, and stock levels
-- ------------------------------------------------------------
CREATE TABLE public.item (
    typeofpay               character varying(255),
    registeredphonenumber   numeric,
    itemname                character varying(255),
    itemhsn                 character varying(255),
    baseunit                character varying(255),
    secondaryunit           character varying(255),
    category                character varying(255),
    itemcode                character varying(255),
    saleprice               numeric,
    salewithorwithouttax    character varying(50),
    discountonsaleprice     numeric,
    percentageoramount      numeric,
    wholesaleprice          numeric,
    wholesalewithorwithouttax character varying(50),
    minimumwholesalequantity numeric,
    purchaseprice           numeric                 DEFAULT 0,
    purchasewithorwithouttax character varying(50),
    taxrate                 character varying(50),
    openingquantity         numeric,
    atprice                 numeric,
    asofdate                date,
    minimumstocktomaintain  numeric,
    _location               character varying(255),
    conversionrates         numeric,
    remainingquantity       numeric,
    percentageoramounttype  character varying(255),
    mrp                     numeric                 DEFAULT 0
);


-- ------------------------------------------------------------
-- TABLE: transactions
-- All sale, purchase, and payment transaction headers
-- ------------------------------------------------------------
CREATE TABLE public.transactions (
    transaction_id          numeric                 NOT NULL DEFAULT nextval('public.transactions_transaction_id_seq'::regclass),
    typeofpay               character varying(255),
    invoicenumber           numeric,
    invoicedate             date,
    stateofsupply           character varying(255),
    paymenttype             character varying(255),
    total                   numeric,
    received                numeric,
    balance                 numeric,
    customername            character varying(255),
    phonenumber             numeric,
    registeredphonenumber   numeric,
    billingaddress          character varying(255),
    shippingaddress         character varying(255),
    paymentstatus           character varying,
    showtransaction         character varying(100)  DEFAULT 'SHOW'::character varying,
    linkedaccount           character varying(255)  DEFAULT 'NOT LINKED'::character varying,
    linkedamount            numeric                 DEFAULT 0,
    paymentinoutinvoicedate date,
    isconverted             boolean                 DEFAULT false,
    paymentininvoicenumber  numeric,
    amountdetails           character varying(255)  DEFAULT ''::character varying,
    bankscustomername       character varying(50)   DEFAULT ''::character varying,
    isbankscustomernameupdate boolean               DEFAULT true
);


-- ------------------------------------------------------------
-- TABLE: item_details
-- Line items (individual products) inside each transaction
-- ------------------------------------------------------------
CREATE TABLE public.item_details (
    detail_id               integer                 NOT NULL DEFAULT nextval('public.item_details_detail_id_seq'::regclass),
    transaction_id          numeric,
    item                    character varying(255),
    qty                     numeric,
    unit                    character varying(50),
    priceperunit            numeric,
    registeredphonenumber   numeric,
    invoicenumber           numeric,
    customername            character varying,
    invoicedate             date,
    typeofpay               character varying,
    paymentstatus           character varying,
    showtransaction         character varying(100)  DEFAULT 'SHOW'::character varying,
    taxrate                 character varying(255)  DEFAULT ''::character varying,
    taxrateamount           numeric                 DEFAULT 0,
    discountpercent         numeric                 DEFAULT 0,
    discountamount          numeric                 DEFAULT 0,
    mrp                     numeric                 DEFAULT 0,
    itemcode                character varying(20)   DEFAULT ''::character varying
);


-- ------------------------------------------------------------
-- TABLE: bankform
-- Bank accounts and fund transfer records
-- ------------------------------------------------------------
CREATE TABLE public.bankform (
    accountdisplayname      character varying(60)   NOT NULL,
    openingbalance          numeric,
    asofdate                date                    NOT NULL,
    transfertype            character varying(50),
    fromaccount             character varying(60),
    toaccount               character varying(60),
    amount                  numeric,
    adjustmentdate          date,
    accountname             character varying(60),
    adjustmenttype          character varying(50),
    description             text,
    typeofpay               character varying(20),
    registeredphonenumber   numeric,
    transaction_id          numeric
);


-- ------------------------------------------------------------
-- TABLE: payementinouttransactions
-- Links payment-in/out invoices to sale/purchase invoices
-- ------------------------------------------------------------
CREATE TABLE public.payementinouttransactions (
    invoicedate             date,
    invoicenumber           numeric,
    typeofpay               character varying(50),
    linkedamount            numeric,
    customername            character varying(50),
    registeredphonenumber   numeric,
    paymentininvoicenumber  numeric,
    showtype                integer                 DEFAULT 0
);


-- ------------------------------------------------------------
-- TABLE: countrow
-- Utility/helper table (used internally for row counting)
-- ------------------------------------------------------------
CREATE TABLE public.countrow (
    count                   bigint
);


-- ============================================================
-- SECTION 3: PRIMARY KEY CONSTRAINTS
-- ============================================================

ALTER TABLE ONLY public.transactions
    ADD CONSTRAINT transactions_pkey PRIMARY KEY (transaction_id);

ALTER TABLE ONLY public.item_details
    ADD CONSTRAINT item_details_pkey PRIMARY KEY (detail_id);


-- ============================================================
-- SECTION 4: FOREIGN KEY CONSTRAINTS
-- ============================================================

ALTER TABLE ONLY public.item_details
    ADD CONSTRAINT item_details_transaction_id_fkey
    FOREIGN KEY (transaction_id) REFERENCES public.transactions(transaction_id);


-- ============================================================
-- SECTION 5: STORED PROCEDURES
-- ============================================================

-- ------------------------------------------------------------
-- SP: sp_addupdatecategory
-- Insert a new category OR rename an existing one.
-- Also renames the category on all items when updated.
-- Params: old name, new name, registered phone number
-- Out:    result message
-- ------------------------------------------------------------
CREATE PROCEDURE public.sp_addupdatecategory(
    IN  v_oldcategory           character varying,
    IN  v_newcategory           character varying,
    IN  v_registeredphonenumber numeric,
    OUT output_result           character varying
)
LANGUAGE plpgsql
AS $$
DECLARE
    COUNTROW  INTEGER;
    COUNTNUM  INTEGER;
    INSERTVAL character varying;
BEGIN
    SELECT COUNT(*) INTO COUNTROW
    FROM category
    WHERE category = v_newcategory AND registeredphonenumber = v_registeredphonenumber;

    SELECT COUNT(*) INTO COUNTNUM
    FROM category
    WHERE category = v_oldcategory AND registeredphonenumber = v_registeredphonenumber;

    IF COUNTNUM != 0 THEN
        IF COUNTROW != 0 THEN
            output_result := 'Record exists';
        ELSE
            UPDATE category
            SET category = v_newcategory
            WHERE category = v_oldcategory AND registeredphonenumber = v_registeredphonenumber;

            UPDATE item
            SET category = v_newcategory
            WHERE category = v_oldcategory AND registeredphonenumber = v_registeredphonenumber;

            output_result := 'Record updated Successfully';
            INSERTVAL := 'NO';
        END IF;
    ELSIF COUNTROW = 0 THEN
        INSERTVAL := 'YES';
    ELSE
        output_result := 'Record exists';
    END IF;

    IF COUNTROW = 0 AND INSERTVAL = 'YES' THEN
        INSERT INTO category(category, registeredphonenumber)
        VALUES (v_newcategory, v_registeredphonenumber);
        output_result := 'Record added Successfully';
    END IF;
END;
$$;


-- ------------------------------------------------------------
-- SP: sp_addupdatepartygroup
-- Insert a new party group OR rename an existing one.
-- Also renames the group on all parties when updated.
-- Params: old name, new name, registered phone number
-- Out:    result message
-- ------------------------------------------------------------
CREATE PROCEDURE public.sp_addupdatepartygroup(
    IN  v_oldgroupname          character varying,
    IN  v_newgroupname          character varying,
    IN  v_registeredphonenumber numeric,
    OUT output_result           character varying
)
LANGUAGE plpgsql
AS $$
DECLARE
    COUNTROW  INTEGER;
    COUNTNUM  INTEGER;
    INSERTVAL character varying;
BEGIN
    SELECT COUNT(*) INTO COUNTROW
    FROM partygroup
    WHERE partygroup = v_newgroupname AND registeredphonenumber = v_registeredphonenumber;

    SELECT COUNT(*) INTO COUNTNUM
    FROM partygroup
    WHERE partygroup = v_oldgroupname AND registeredphonenumber = v_registeredphonenumber;

    IF COUNTNUM != 0 THEN
        IF COUNTROW != 0 THEN
            output_result := 'Record exists';
        ELSE
            UPDATE partygroup
            SET partygroup = v_newgroupname
            WHERE partygroup = v_oldgroupname AND registeredphonenumber = v_registeredphonenumber;

            UPDATE party
            SET partygroup = v_newgroupname
            WHERE partygroup = v_oldgroupname AND registeredphonenumber = v_registeredphonenumber;

            output_result := 'Record updated Successfully';
            INSERTVAL := 'NO';
        END IF;
    ELSIF COUNTROW = 0 THEN
        INSERTVAL := 'YES';
    ELSE
        output_result := 'Record exists';
    END IF;

    IF COUNTROW = 0 AND INSERTVAL = 'YES' THEN
        INSERT INTO partygroup(partygroup, registeredphonenumber)
        VALUES (v_newgroupname, v_registeredphonenumber);
        output_result := 'Record added Successfully';
    END IF;
END;
$$;


-- ------------------------------------------------------------
-- SP: sp_findorinsertitems
-- Core transaction helper. On each transaction save/update:
--   1. Auto-creates party if not found
--   2. Auto-creates item if not found
--   3. Adjusts item stock (add for purchase/sale return, deduct for sale)
--   4. Updates party's payable/receivable balances
-- Params: customer info, item info, qty, invoice details, isUpdate flag
-- Out:    result message
-- ------------------------------------------------------------
CREATE PROCEDURE public.sp_findorinsertitems(
    IN  v_customername          character varying,
    IN  v_phonenumber           numeric,
    IN  v_registeredphonenumber numeric,
    IN  v_billingaddress        character varying,
    IN  v_shippingaddress       character varying,
    IN  v_topayparty            numeric,
    IN  v_toreceivefromparty    numeric,
    IN  v_item                  character varying,
    IN  v_qty                   numeric,
    IN  v_remainingquantity     numeric,
    IN  v_typeofpay             character varying,
    IN  v_invoicenumber         numeric,
    IN  v_isupdate              boolean,
    OUT output_result           character varying
)
LANGUAGE plpgsql
AS $$
DECLARE
    countRow INTEGER;
    countNum INTEGER;
    countQty NUMERIC;
BEGIN
    BEGIN
        -- Auto-insert party if not already present
        SELECT COUNT(*) INTO countRow
        FROM party
        WHERE partyname = v_customername AND registeredphonenumber = v_registeredphonenumber;

        IF countRow = 0 THEN
            INSERT INTO party(partyname, phonenumber, registeredphonenumber, billingaddress, shippingaddress, partygroup)
            VALUES (v_customername, v_phonenumber, v_registeredphonenumber, v_billingaddress, v_shippingaddress, 'GENERAL');
        END IF;

        -- Auto-insert item if not already present
        SELECT COUNT(*) INTO countNum
        FROM item
        WHERE itemname = v_item AND registeredphonenumber = v_registeredphonenumber;

        -- For update: find previously saved qty for this line item
        SELECT qty INTO countQty
        FROM item_details
        WHERE customername = v_customername
          AND typeofpay    = v_typeofpay
          AND item         = v_item
          AND invoicenumber = v_invoicenumber;

        -- Adjust qty delta when updating (avoid double-deducting)
        IF v_isupdate THEN
            IF countQty = v_qty THEN
                v_qty := 0;
            ELSE
                v_qty := v_qty - countQty;
            END IF;
        END IF;

        IF countNum = 0 THEN
            -- Item doesn't exist yet — create it with opening stock
            INSERT INTO item(itemname, remainingquantity, registeredphonenumber, typeofpay, category)
            VALUES (v_item, v_remainingquantity - v_qty, v_registeredphonenumber, v_typeofpay, 'GENERAL');
        ELSE
            -- Item exists — adjust stock based on transaction direction
            IF v_typeofpay = 'SALE RETURN' OR v_typeofpay = 'PURCHASE' THEN
                UPDATE item
                SET remainingquantity = remainingquantity + v_qty
                WHERE itemname = v_item AND registeredphonenumber = v_registeredphonenumber;
            ELSE
                UPDATE item
                SET remainingquantity = remainingquantity - v_qty
                WHERE itemname = v_item AND registeredphonenumber = v_registeredphonenumber;
            END IF;
        END IF;

        -- Sync party balances
        UPDATE party
        SET topayparty = v_topayparty, toreceivefromparty = v_toreceivefromparty
        WHERE partyname = v_customername AND registeredphonenumber = v_registeredphonenumber;

        output_result := 'Record added or updated successfully';
    EXCEPTION
        WHEN OTHERS THEN
            output_result := 'An error occurred: ' || SQLERRM;
    END;
END;
$$;


-- ============================================================
-- END OF SCHEMA SCRIPT
-- ============================================================
