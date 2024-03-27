------------------------------------------------------------------------------------------------------------------------
-- Create tables
create table project
(
    id   integer not null
        constraint project_pk
            primary key,
    name varchar default ''::character varying
);

alter table project
    owner to admin;

create table issue
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

create table page
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

create table profile
(
    entry varchar not null
        constraint profile_pk
            primary key,
    value varchar
);

alter table profile
    owner to admin;

------------------------------------------------------------------------------------------------------------------------
-- Inset default rows.
insert into public.profile (entry, value) values ('ES_USER', 'elastic');
insert into public.profile (entry, value) values ('ES_URL', 'http://localhost:9200');
insert into public.profile (entry, value) values ('ES_PASSWORD', 'changeme');
insert into public.profile (entry, value) values ('REDMINE_URL', 'http://localhost');
insert into public.profile (entry, value) values ('NOTION_DB_ID', '00000000-0000-0000-0000-000000000000');
insert into public.profile (entry, value) values ('REDMINE_API_KEY', 'changeme');
insert into public.profile (entry, value) values ('NOTION_API_KEY', 'changeme');
insert into public.profile (entry, value) values ('LOADER_TARGET_PROJECT_IDS', '0');
insert into public.profile (entry, value) values ('NOTION_API_URL', 'https://api.notion.com/v1');
insert into public.profile (entry, value) values ('NOTION_API_VERSION', '2022-06-28');
insert into public.profile (entry, value) values ('LOADER_RECOVER_DURATION', '90');
insert into public.profile (entry, value) values ('FIXER_MIN_ISSUE_LIMIT', '0');
insert into public.profile (entry, value) values ('FIXER_MAX_ISSUE_LIMIT', '100000');
