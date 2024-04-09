
NotionPublisher 유틸리티 제품군이 사용하는 API 서비스. NP 제품군의 각종 DB 작업은 이 API를 통해서 수행됩니다.

---
## Environments
* `DB_HOST`: Postgres 호스트
* `DB_PORT`:  Postgres 포트
* `DB_USER`: Postgres 로그인 유저
* `DB_PASSWORD`: Postgres 유저의 패스워드
* `DB_NAME`: Postgres Database 이름
* `ENCRYPT_KEY`: 암호화 키 값. 16자리여야 합니다.
* `ENABLE_SWAGGER`: Swagger를 강제로 활성화 시킬지 여부.

## Deploy
* [docker-compose.yml](../Sup.Np.Api/docker-compose.yml)을 수정합니다.
	* 만약 [appsettings.json](../Sup.Np.Api/appsettings.json)의 설정값을 사용하고 싶다면 volumes 영역을 활성화 시킨 채 명령어를 수행합니다. 단, 이 경우 [appsettings.json](../Sup.Np.Api/appsettings.json)의 설정을 수정해야 합니다.
* `docker-compose up -d` 명령어를 사용해 실행합니다.

