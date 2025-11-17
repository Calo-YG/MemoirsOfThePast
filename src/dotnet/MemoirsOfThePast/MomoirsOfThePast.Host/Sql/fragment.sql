CREATE TABLE fragment(
    id           UUID PRIMARY KEY,
    memory_id    UUID NOT NULL,          -- 关联记忆ID（外键）
    description  TEXT,                   -- 保留"回忆藐视"注释（实际为描述内容）
    occur_date   TIMESTAMPTZ NOT NULL,   -- 事件发生时间（精确到时区）
    location     TEXT,                   -- 事件发生地点
    scene        TEXT,                   -- 事件场景描述
    create_date  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- 添加外键约束（关联到memory_entities表）
ALTER TABLE fragment
ADD CONSTRAINT fk_fragment_to_memory
FOREIGN KEY (memory_id) REFERENCES memory(id) ON DELETE CASCADE;

-- 添加中文注释（精确对应业务含义）
--COMMENT ON TABLE fragment IS '回忆碎片表（存储记忆的细节片段）';
--COMMENT ON COLUMN fragment.id IS '碎片ID（UUID主键）';
--COMMENT ON COLUMN fragment.memory_id IS '关联的记忆ID（外键，级联删除）';
--COMMENT ON COLUMN fragment.description IS '回忆内容描述（保留"回忆藐视"原始注释含义）';
--COMMENT ON COLUMN fragment.occur_date IS '事件发生时间（精确到时区，非创建时间）';
--COMMENT ON COLUMN fragment.location IS '事件发生地点（地理信息）';
--COMMENT ON COLUMN fragment.scene IS '事件场景描述（环境上下文）';
--COMMENT ON COLUMN fragment.create_date IS '碎片创建时间（系统自动生成）';

-- 创建优化索引
CREATE INDEX idx_fragment_by_memory ON fragment_entities(memory_id);
CREATE INDEX idx_fragment_by_occur_date ON fragment_entities(occur_date DESC);