CREATE TABLE memory (
    id          UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name        TEXT NOT NULL,
    description  TEXT,        -- 保留原始拼写错误（与C# Description 属性完全一致）
    avatar      TEXT,
    background  TEXT,
    prompt      TEXT,
    create_date TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 添加中文注释（精确对应字段含义）
--COMMENT ON TABLE memory_entities IS '独立记忆实体表（不依赖角色表）';
--COMMENT ON COLUMN memory_entities.id IS '记忆ID（UUID主键）';
--COMMENT ON COLUMN memory_entities.name IS '记忆关联的人物名称（独立存储，非外键）';
--COMMENT ON COLUMN memory_entities.descrption IS '记忆描述内容（保留原始拼写以匹配C#属性）';
--COMMENT ON COLUMN memory_entities.avatar IS '记忆专属头像（可能与角色头像不同）';
--COMMENT ON COLUMN memory_entities.background IS '记忆生成的上下文背景';
--COMMENT ON COLUMN memory_entities.prompt IS 'AI生成的记忆提示词（定时任务填充）';
--COMMENT ON COLUMN memory_entities.create_date IS '记忆创建时间（带时区）';

-- 添加查询优化索引
CREATE INDEX idx_memory ON memory(create_date DESC);