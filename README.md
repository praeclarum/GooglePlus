The Google+ API
===

Below is documentation on how to access Google+ resources. It's based on the access patterns of the
browser.

What you should be impressed most about this API is how large some of these
data objects are. For instance, the `Item` object below which represents an
item in your stream has **23 documented fields** while the Google code
has room for **91 fields**. Who knows what they have in store for us next.


1 Authentication
===

Authentication is performed by simply POSTing your credentials to a service and tracking cookies. To
kick it off, grab some cookies from:

1.1 GET http://plus.google.com
---

We do this just to collect some cookies and the **gpcaz** variable in the
response URL.

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

Every Item has an `ItemType` which can be one of:

* **Photos** Photos and Profile Photos
* **Buzz** Text Post


2.1 Common Data
---

    Date = [
        0: Day: int
        1: Month: int
        2: Year: int
        3: Formatted: string
    ]
    
    Job = [
        0: Company: string
        1: Title: string
        2: Dates: Date[]
    ]

    School = [
        0: Name: string
        1: Degree: string
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
    
    Voter = [
        0: FullName
        1: UserId: string
        2: UserUrl: url
        3: UserPhotoUrl: url
    ]
    
    Voting = [
        0: UrlId: string
        1: VoteType: int // 4 = post, 5 = comment
        16: TotalVotes: int
        17: KnownVoters: Voter[]
    ]
    
    Comment = [    
        1: UserFullName: string
        2: FullTextHtml: string
        3: ItemCommentsId: string
        4: CommentInCommentsUrlId: string
        5: FullTextRaw: string
        6: UserId: string
        7: CommentUrlId: string
        10: UserUrl: relative url
        15: Voting
        16: UserPhotoUrl: url    
    ]
    
    Thumbnail = [
        1: ResourceUrl: url
        2: Height: int
        3: Width: int
    ]
    
    Media = [
        1: Url: url
        3: ContentType: string
        4: ShortContentType: string
    ]
    
    MetadataEntry = [
        1: Value
        2: Key
    ]
    
    Attachment = [
        5: Preview: Thumbnail
        21: SomeImageUrl: url
        24: Media
        41: Thumbnails: Thumbnail[]
        47: Metadata: MetadataEntry[]
    ]
    
    HiddenComments = [
        0: Count
        1: Commentators: string[]
        2: HiddenCommentsId
        2: ItemCommentsId
    ]
    
    UserRef = [
        0: FullName: string
        1: UserId: string
        5: UserUrl: relative url
    ]
    
    Item = [
        0: Up: string
        2: ItemType: string
        3: UserFullName: string
        4: FullTextHtml: string
        5: ItemId: string
        6: FaviconUrl: url
        7: Comments: Comment[]
        8: ItemUrlId: string
        10: Tags: string /* colon separated */
        11: Attachments: Attachment[]
        14: FullTextIntermediate: string
        16: UserId: string
        18: PhotoUrl: url
        20: FullTextRaw: string
        21: PostUrl: relative url
        22: SomeId: string
        24: UserUrl: relative url
        25: RelatedUsers: UserRef[]
        30: ItemCommentsId: string
        33: PreviewHtml: string
        60: HiddenComments
        73: Voting
        84: SomeDomain: string = "social.google.com"        
    ]
    
    Circle = [
        0: [
            0: CircleId: string
        ]
        1: [
            0: Name: string
            2: Description: string
            12: UrlId: string
            13: Order: int
        ]
    ]
    
    PagingInfo = [
        0: Page: int // 1-based
        1: NumPages: int
        5: ItemsPerPage: int
        7: Domain: string
    ]


2.2 Home Data
---
    
    HomeDate = {
        "2": LoggedInUserData
        "4": StreamItemsData
        "12": CirclesData
    }

    LoggedInUserData = [        
        0: UserId: string
        1: UserData
        2: Email: string
        4: Culture: string
        20: HumanReadableEmail: string
        21: AllUrl: relative url
    ]
    
    StreamItemsData = [
        0: Items: Item[]
        2: PagingInfo
    ]
    
    CirclesData = [
        0: Circles: Circle[]
    ]

    
    



























