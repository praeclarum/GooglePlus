The Google+ API
===

Below is documentation on how to access Google+ resources. It's based on the access patterns of the
browser.

1 Authentication
===

Authentication is performed by simply POSTing your credentials to a service and tracking cookies. To
kick it off, grab some cookies from:

1.1 GET http://plus.google.com
---

We do this just to collect some cookies.

1.2 GET https://www.google.com/accounts/ServiceLogin?service=oz&continue=https://plus.google.com/?gpcaz%3DXXXXXXXX&ltmpl=es2st&hideNewAccountLink=1&hl=en-US
---

We need some more cookies; specifically, **GALX** which gets set thanks to this call. You will need
to substitute **gpcaz** with the one you got from 1.1.

1.3 POST https://www.google.com/accounts/ServiceLoginAuth
---

We pass our credentials to this service to authenticate.

    Content-Type	application/x-www-form-urlencoded

    ltmpl		es2st
    pstMsg		1
    dnConn		https://accounts.youtube.com
    continue	https://plus.google.com/?gpcaz=XXXXXXXX
    service		oz
    hl			en-US
    timeStmp	
    secTok	
    GALX		# From cookies in 1.2
    Email		# Email address
    Passwd		# Password
    PersistentCookie	yes
    rmShown		1
    signIn		Sign in
    asts

On a successful POST, you will get back the G+ home page. That page contains a lot of
JavaScript data in `<script>` tags. The next section covers some of that data.

2 The API Data
===

2.1 Common Data
---

    Date = [
        0: Day: int
        1: Month: int
        2: Year: int
        3: Formatted: string
    ]
    
    Job = [
        0: Company
        1: Title
        2: Dates: Date[]
    ]

    School = [
        0: Name
        1: Degree
        2: Dates: Date[]
    ]
    
    Link = [
        1: Url: url
        2: FaviconUrl: relative url
        3: Title: string
        6: AllUrls: url[]        
    ]

    UserData = [    
        2: UserUrl: url
        3: PhotoUrl: url
        4: [
            1: FirstName: string
            2: LastName: string
            3: FullName: string
        ]
        6: [
            1: Profession: string
        ]
        7: [
            1: Jobs: Job[]
        ]
        8: [
            1: Education: School[]
        ]
        9: [
            1: Location: string
            2: PastLocations: string[]
        ]
        10: LocationMapUrl: relative url
        11: [
            0: UserLinks: Link[]
        ]
        30: UserId: string
        33: [
            1: ProfileString: string
        ]
        44: Flags: int[]
        47: [
            1: ShortName: string
        ]        
    ]


2.2 Home Data
---
    
    HomeDate = {
        "2": LoggedInUserData
    }

    LoggedInUserData = [        
        0: UserId: string
        1: UserData
        2: Email: string
        
        4: Culture: string
        20: HumanReadableEmail: string
        21: AllUrl: relative url
    ]

    
    



























