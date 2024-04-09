
Redmine으로 부터 이슈를 불러와 최신 상태로 유지 시키는 역할을 하는 제품.

---

## Environments
* `API_HOST`: [Sup.Np.Api](./Sup.Np.Api)의 호스트
* `API_PORT`:  [Sup.Np.Api](./Sup.Np.Api)의 포트
* `ENCRYPT_KEY`: 암호화 키 값. 16자리여야 합니다.

## Deploy
* [docker-compose.yml](../Sup.Np.IssueLoader/docker-compose.yml)을 수정합니다.
	* 만약 [appsettings.json](../Sup.Np.IssueLoader/appsettings.json)의 설정값을 사용하고 싶다면 volumes 영역을 활성화 시킨 채 명령어를 수행합니다. 단, 이 경우 [appsettings.json](../Sup.Np.IssueLoader/appsettings.json)의 설정을 수정해야 합니다.
* `docker-compose up -d` 명령어를 사용해 실행합니다.