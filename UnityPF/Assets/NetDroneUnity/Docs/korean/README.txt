#
# 1. 패키지 구성 소개
#
Client
- Fonts (클라이언트 예제용 폰트)
- Materials (Origin Studio 로고)
- Resources - binary (bytes 포멧 데이터 파일)
           - text (csv 포멧 데이터 파일)
- Scenes (클라이언트 예제용 씬)
- Scripts (씬과 연결된 소스 코드)
          - DB (csv, bytes 포멧 파일별 로더)
          - Net (프로토콜별 패킷 처리 기능)
                - Data (NDP로 주고받을 데이터 구조체)
                - Send (패킷 전송용 처리 기능)
          - State (클라이언트 상태 처리 기능)
          - UI (메세지 처리 기능)
          - Unit (플레이어 유닛 처리와 관리 기능)
          - Util (NDCF 처리 기능)
          - World (채널/룸 처리 기능)
- SDK
          - netdroneunity
                    - db (csv, bytes 포멧 데이터 로더)
                    - net (Poll을 기반으로한 NDP 클라이언트)
                    - util (Singleton 정의 및 서버 시간 관리자)
- Textures (Origin Studio 로고)

Docs (관련 문서)

Server (넷드론 엔진의 유니티 서버 예제)
- hyperserver (PHP 기반 서버)
- studyserver (C# 기반 서버)
- unityserver (C++ 기반 서버)

Tools
- DataConverter (csv to bytes)
- CryptTester (text or packet)
- DummyClient (tcp or rudp)
- ReuseMemory (allocation test)

#
# 2. GameFramework 사용 안내
#
GameFramework란 C#에서 C++ 처럼 전역적 클래스/함수 작성을 지원할수 있도록 고안된 partial class 집합입니다.

그외 서버 시간을 보관/처리 하는 Tick 클래스, csv와 bytes 포멧을 지원하는 DB 관리자, 표준 Singleton 클래스 등이 포함됩니다.

UnityEngine 네임스페이스에 포함되어있는 GameFramework는 using UnityEngine을 선언할 경우 별다른 추가 선언이 필요없습니다.

GameFramework를 사용하는 작성 방법은 다음과 같은 항목입니다.

* MonoBehaviour를 사용하는 기존 클래스 대체 방법
client/Defines.cs를 참고하시면 GameFramework는 MonoBehaviour 상속이 되어있습니다.
유니티 에티터용 스크립트 작성시 MonoBehaviour 대신 GameFramework를 상속하세요.

public class Example : GameFramework {
...
}

이제 partial class GameFramework로 묶여있는 모든 클래스/함수들은 네임스페이스 접근없이 전역적 접근이 가능합니다.

* 전역적 접근 구조에 포함시키고 싶은 일반 클래스 작성 방법
넷드론 유니티 소스 코드와 클라이언트 예제는 모두 partial class GameFramework 안에 존재합니다.
새로운 클래스 및 전역함수 작성을 하시려면 아래 예제들 처럼 작성하세요.

일반 전역 클래스 예제:
namespace UnityEngine {
	public partial class GameFramework {
		public class CExample {
			public CExample() {}
			~CExample() {}

			...
		}
	}
}

싱글톤 전역 클래스 예제:
namespace UnityEngine {
	public partial class GameFramework {
		public static CExample g_kExample { get { return CExample.GetInstance(); } }

		public class CExample : CSingleton<CExample> {
			public CExample() {}
			~CExample() {}

			public void Test() {}
			...
		}
	}
}

이제 partical class GameFramework에 포함되어 있거나 public class Example : GameFramework 처럼 상속되는 모든 곳에서 g_kExample.Test()만 입력하면 네임스페이스 접근없이 호출됩니다.

* 명시적 SAFE_DELETE를 선언하는 이유
많은 개발자들의 고민중 한가지가 바로 GC.Collect() 함수의 강제적 호출입니다.
이는 전체 객체를 대상으로 하는 강제적 수집/제거 기능이어서 성능저하 문제가 야기됩니다.

netdroneunity/net/Command.cs의 함수들과 같이 Marshal.AllocHGlobal, Marshal.FreeHGlobal을 활용하는 방법도 있겠지만 특수한 상황에만 적용하시고 아래 두가지 사항 준수가 필요합니다.

- 클래스 구조를 설계할때 GC.Collect() 함수 실행을 최소화 할수 있도록 되도록 재사용성에 중점을 두기 바랍니다.
- 더이상 사용하지 않는 객체들에들에 대한 Clear, Close 함수 호출및 null 지정을 반드시 해야합니다.

GameFramework에서는 객체에 대한 해제 처리 및 null 지정을 명확히 실행하기 위해 C++ 매크로 선언과 비슷한 방식의 SAFE_DELETE, SAFE_DELETE_ARRAY, SAFE_DELETE_RELEASE 함수를 사용합니다.

netdroneunity/Defines.cs 와 Scripts/Defines.cs 를 참고하세요.

#
# 3. Hierarchy 계층 구조 설명
#
넷드론 엔진의 유니티 버전 예제는 GameObject를 생성하여 계층구조 및 상세 정보를 Hierarchy에 표시합니다.

표시하는 내용은 아래와 같습니다.

Networks (Text 컴포넌트에 Packet, List 정보 출력)
         - Master
                  - Queue (Native:Count)
                  - Connector (Main:TCP)
                  - Connector (Sub:TCP)
         - Slave
                  - Queue (Native:Count)
                  - Connector (Main:UDP)
                  - Listener (Main:UDP)

Channels
         - User List
                     - user name (Key)
                     ...
         - Room List
                     - room name (Room Id)
                     ...

Main State (State Type)

Main Room (Room Id)
          - Other Player (Key)
          ...

Main Player (Key)

다중 접속 테스트 방법은 아래와 같습니다.

* Build Settings > PC, Mac & Linux Standalone 으로 빌드한뒤 여러개의 테스트 클라이언트 실행후 각각다른 아이디로 로그인
* 유니티 에디터에서도 ExampleMenu 실행후 로그인
* 동일한 채널일 경우 Room을 생성하면 생성된 Room Id가 Broadcast 됨
* 모든 클라이언트들을 해당 Room Id로 Join 함

이제 Hierarchy에 위와 같은 계층 구조가 표시됩니다.
예제에 포함된 기능들을 테스트 하면서 Hierarchy에 어떻게 표시되는지 살펴보세요.

#
# 4. 로그 및 메세지 출력 방법
#
넷드론 엔진의 유니티 버전은 TRACE, OUTPUT, PRINT, ERROR 함수를 정의하여 표준 출력 내용을 표시합니다.

TRACE: DISABLE_UNITY 선언이 있을경우 Console.WriteLine(FILE, LINE, FUNCTION) 함수와 연결, 선언이 없을경우 UnityEngine.Debug.Log 함수와 연결됩니다.
OUTPUT: DISABLE_UNITY 선언이 있을경우 Console.WriteLine(FILE, LINE, FUNCTION) 함수와 연결, 선언이 없을경우 UnityEngine.Debug.Log 함수와 연결됩니다.
PRINT: DISABLE_UNITY 선언이 있을경우 Console.WriteLine 함수와 연결, 선언이 없을경우 UnityEngine.Debug.Log 함수와 연결됩니다.
ERROR: DISABLE_UNITY 선언이 있을경우 System.Diagnostics.Debug.WriteLine 함수와 연결, 선언이 없을경우 UnityEngine.Debug.LogError 함수와 연결됩니다.

TRACE 명령어를 사용하려면 Assets 폴더 아래 gmcs.rsp/smcs.rsp (-define:DEBUG) 파일을 생성하세요.
DEBUG 선언을 제거할 경우 TRACE 코드는 모두 사라집니다.

#
# 5. 넷드론 엔진의 확장 사용 방법
#
넷드론 엔진의 유니티 버전은 유니티 에디터 뿐만 아니라 C#으로 구현하는 모든 프로젝트에 적용하는 것이 가능합니다.
유니티 에디터가 아닌곳에서 사용하려면 DISABLE_UNITY 선언을 추가하세요.

다음은 MonoDevelop에서 선언을 추가하는 위치입니다.

Project Options: Build > Compiler > Define Symbols

그외 기술지원 관련 자료는 wiki.vogie.net 에서 얻을 수 있으며, www.vogie.net 에서 토론이 가능합니다.