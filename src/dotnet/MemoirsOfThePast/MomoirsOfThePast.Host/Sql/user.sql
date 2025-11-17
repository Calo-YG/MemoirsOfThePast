CREATE TABLE users (
    id          UUID PRIMARY KEY,
    name        TEXT NOT NULL,              -- 用户昵称
    avatar      TEXT,                       -- 头像URL
    account     TEXT NOT NULL UNIQUE,       -- 登录账号（唯一约束）
    password    TEXT NOT NULL,              -- 密码哈希（非明文）
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_account ON users(account);
CREATE INDEX idx_users_name ON users(name);
CREATE INDEX idx_users_create_date ON users(create_date DESC);