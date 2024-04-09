
Notion에 페이지를 수동으로 작성한 경우 해당 페이지 정보를 DB에 저장하는 제품.

---

## Environments
* `API_HOST`: [Sup.Np.Api](./Sup.Np.Api)의 호스트
* `API_PORT`:  [Sup.Np.Api](./Sup.Np.Api)의 포트

## Deploy
* Database `profile` 테이블의 `FIXER_MAX_ISSUE_LIMIT`와 `FIXER_MIN_ISSUE_LIMIT`를 수정합니다.
* [docker-compose.yml](../Sup.Np.PageFixer/docker-compose.yml)을 수정합니다.
	* 만약 [appsettings.json](../Sup.Np.PageFixer/appsettings.json)의 설정값을 사용하고 싶다면 volumes 영역을 활성화 시킨 채 명령어를 수행합니다. 단, 이 경우 [appsettings.json](../Sup.Np.PageFixer/appsettings.json)의 설정을 수정해야 합니다.
* `docker-compose up -d` 명령어를 사용해 실행합니다.