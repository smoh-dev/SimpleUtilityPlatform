------------------------------------------------------------------------------------------------------------------------
-- Create tables
create table if not exists project
(
    id   integer not null
        constraint project_pk
            primary key,
    name varchar default ''::character varying
);

alter table project
    owner to admin;

create table if not exists issue
(
    id              integer not null
        constraint issue_pk
            primary key,
    project_id      integer
        constraint issue_project_id_fk
            references project,
    type            varchar default ''::character varying,
    status          varchar default ''::character varying,
    priority        varchar(32),
    assigned_to     varchar default ''::character varying,
    target_version  varchar default ''::character varying,
    category_name   varchar default ''::character varying,
    parent_issue_id integer,
    title           varchar default ''::character varying,
    created_on      timestamp with time zone,
    updated_on      timestamp with time zone,
    last_posted_on  timestamp with time zone,
    author          varchar
);

alter table issue
    owner to admin;

create table if not exists page
(
    id         varchar                  not null
        constraint page_pk
            primary key,
    issue_id   integer                  not null,
    posted_at  timestamp with time zone not null,
    account_id integer                  not null,
    constraint page_pk_2
        unique (account_id, issue_id)
);

alter table page
    owner to admin;

create table if not exists profile
(
    entry varchar not null
        constraint profile_pk
            primary key,
    value varchar
);

alter table profile
    owner to admin;

create table if not exists license
(
    key           varchar not null
        constraint license_pk
            primary key,
    product       varchar not null,
    auth_audience varchar,
    signing_key   varchar
);

alter table license
    owner to admin;

create index if not exists license_product_index
    on license (product);

------------------------------------------------------------------------------------------------------------------------
-- Inset default rows.
insert into profile (entry, "value") values ('AWS_KMS_KEY_ID', '');
insert into profile (entry, "value") values ('ES_PASSWORD', '');
insert into profile (entry, "value") values ('ES_URL', '');
insert into profile (entry, "value") values ('ES_USER', '');
insert into profile (entry, "value") values ('FIXER_MAX_ISSUE_LIMIT', '');
insert into profile (entry, "value") values ('FIXER_MIN_ISSUE_LIMIT', '');
insert into profile (entry, "value") values ('LOADER_RECOVER_DURATION', '');
insert into profile (entry, "value") values ('LOADER_SCHEDULE', '');
insert into profile (entry, "value") values ('LOADER_TARGET_PROJECT_IDS', '');
insert into profile (entry, "value") values ('NOTION_API_KEY', '');
insert into profile (entry, "value") values ('NOTION_API_URL', '');
insert into profile (entry, "value") values ('NOTION_API_VERSION', '');
insert into profile (entry, "value") values ('NOTION_DB_ID', '');
insert into profile (entry, "value") values ('OAUTH_AUTHORITY', '');
insert into profile (entry, "value") values ('OAUTH_AUTHORIZATION_URL', '');
insert into profile (entry, "value") values ('OAUTH_METADATA_URL', '');
insert into profile (entry, "value") values ('OAUTH_TOKEN_URL', '');
insert into profile (entry, "value") values ('PUBLISHER_SCHEDULE', '');
insert into profile (entry, "value") values ('REDMINE_API_KEY', '');
insert into profile (entry, "value") values ('REDMINE_URL', '');