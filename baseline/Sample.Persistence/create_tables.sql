-- =====================================================================
-- Oracle AI Database 26ai (Free / 23.26.x) 用 サンプルスキーマ
-- ユーザー名・パスワードで接続したアプリ用ユーザーのスキーマに作成する想定。
-- 非クォート識別子で作成 → Oracle 内部では ITEM / ITEM_CODE 等の大文字で格納され、
-- BomDbContext の大文字マッピングと一致する。
-- 接続例: User Id=APPUSER;Password=****;Data Source=localhost:1521/FREEPDB1;
-- =====================================================================

-- ---- テーブル定義 -----------------------------------------------------

CREATE TABLE Item (
    item_code  VARCHAR2(20)  NOT NULL,
    item_name  NVARCHAR2(40) NOT NULL,
    CONSTRAINT pk_item PRIMARY KEY (item_code)
);

CREATE TABLE Material (
    item_code  VARCHAR2(20)  NOT NULL,
    item_name  NVARCHAR2(40) NOT NULL,
    CONSTRAINT pk_material PRIMARY KEY (item_code)
);

CREATE TABLE Bom (
    item_code    VARCHAR2(20) NOT NULL,
    m_item_code  VARCHAR2(20) NOT NULL,
    requirement  NUMBER(9,2)  NOT NULL,
    CONSTRAINT pk_bom PRIMARY KEY (item_code, m_item_code),
    CONSTRAINT fk_bom_item     FOREIGN KEY (item_code)   REFERENCES Item(item_code),
    CONSTRAINT fk_bom_material FOREIGN KEY (m_item_code) REFERENCES Material(item_code)
);

-- ---- サンプルデータ(画面イメージと同じ内容) --------------------------

INSERT INTO Item (item_code, item_name) VALUES ('i1', N'品番1');
INSERT INTO Item (item_code, item_name) VALUES ('i2', N'品番2');

INSERT INTO Material (item_code, item_name) VALUES ('m11', N'部品品番11');
INSERT INTO Material (item_code, item_name) VALUES ('m21', N'部品品番21');
INSERT INTO Material (item_code, item_name) VALUES ('m22', N'部品品番22');
INSERT INTO Material (item_code, item_name) VALUES ('m23', N'部品品番23');

INSERT INTO Bom (item_code, m_item_code, requirement) VALUES ('i1', 'm11', 2);
INSERT INTO Bom (item_code, m_item_code, requirement) VALUES ('i2', 'm21', 1);
INSERT INTO Bom (item_code, m_item_code, requirement) VALUES ('i2', 'm22', 3);
INSERT INTO Bom (item_code, m_item_code, requirement) VALUES ('i2', 'm23', 5);

COMMIT;
