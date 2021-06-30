import mockjs from 'mockjs';

const titles = [
  'Max',
  'Jacky',
  'Lucy',
  'Rocky',
  'Kevi',
  'Nico',
  'Blacky',
  'Mickey',
];

const registered = [
  'Representative',
  'Owner',
  'Owner',
  'Representative',
  'Owner'
];

const gender = [
  'Male',
  'Female',
  'Female',
  'Male',
  'Female'
];

const dob = [
  '2019-05-21T08:22:58.128Z',
  '2019-02-21T08:22:58.128Z',
  '2019-03-21T08:22:58.128Z',
  '2019-07-21T08:22:58.128Z',
  '2019-01-21T08:22:58.128Z'
];

const avatars = [
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRwsNhZsjjMQG2fw3r5gtO6FNZock7SMbWRO7x-urW456b55Yyitg', // Alipay
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSwk0g2F9dpuD9n1hzMmx9g5CCMAUmx9RufX6JFJPveYwsFK0kh9A', // Angular
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSaHLj3iDncGYEJ-ZpE9_UKAHgv2agZVNFBlQ1f38q_KKwuwqPDkA', // Ant Design
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSR4X46KBqmcdft5Zr9S48d-2X3H6xAxT5h-64HGeaKh7apHvlC', // Ant Design Pro
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQUr6ehdfL-ypjzmnYN7uxxFEVNmtqOjdiv1TFc0u-A7Ca3fEC7RA', // Bootstrap
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTKiJzuuqTD93wr-rsHzpJOoxtA0z1z6EO3RECebLTPmAoJF6Bezw', // React
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTPrLiYUR0ZaVZPX2acfxwdu76h1WE61RhA31rC3pjLgKRuTH1L', // Vue
  'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTnet1DMhwiyfMwNBj7qxdxJAKcY-y8v02-uaFqRzr7Ct-pTp0', // Webpack
];

const avatars2 = [
  'https://gw.alipayobjects.com/zos/rmsportal/BiazfanxmamNRoxxVxka.png',
  'https://gw.alipayobjects.com/zos/rmsportal/cnrhVkzwxjPwAaCfPbdc.png',
  'https://gw.alipayobjects.com/zos/rmsportal/gaOngJwsRYRaVAuXXcmB.png',
  'https://gw.alipayobjects.com/zos/rmsportal/ubnKSIfAJTxIgXOKlciN.png',
  'https://gw.alipayobjects.com/zos/rmsportal/WhxKECPNujWoWEFNdnJE.png',
  'https://gw.alipayobjects.com/zos/rmsportal/jZUIxmJycoymBprLOUbT.png',
  'https://gw.alipayobjects.com/zos/rmsportal/psOgztMplJMGpVEqfcgF.png',
  'https://gw.alipayobjects.com/zos/rmsportal/ZpBqSxLxVEXfcUNoPKrz.png',
  'https://gw.alipayobjects.com/zos/rmsportal/laiEnJdGHVOhJrUShBaJ.png',
  'https://gw.alipayobjects.com/zos/rmsportal/UrQsqscbKEpNuJcvBZBu.png',
];

const covers = [
  'https://gw.alipayobjects.com/zos/rmsportal/uMfMFlvUuceEyPpotzlq.png',
  'https://gw.alipayobjects.com/zos/rmsportal/iZBVOIhGJiAnhplqjvZW.png',
  'https://gw.alipayobjects.com/zos/rmsportal/iXjVmWVHbCJAyqvDxdtx.png',
  'https://gw.alipayobjects.com/zos/rmsportal/gLaIAoVWTtLbBWZNYEMg.png',
];
const desc = [
  '那是一种内在的东西， 他们到达不了，也无法触及的',
  '希望是一个好东西，也许是最好的，好东西是不会消亡的',
  '生命就像一盒巧克力，结果往往出人意料',
  '城镇中有那么多的酒馆，她却偏偏走进了我的酒馆',
  '那时候我只会想自己想要什么，从不想自己拥有什么',
];

const night = [
  true,
  true,
  false,
  false,
  false
]

const statuss = [
  'Pending',
  'approved',
  'Pending',
  'approved',
  'Pending',
]

const user = [
  'Wu Jiaha',
  'Qu Lili',
  'Zhou Xingxing',
  'Lin Dongdong',
  'Wu Jiaha',
  'Lin Dongdong',
  'Qu Lili',
  'Wu Jiaha',
  'Zhou Xingxing',
  'Lin Dongdong',
];

function fakeList(count) {
  const list = [];
  for (let i = 0; i < count; i += 1) {
    list.push({
      id: `fake-list-${i}`,
      dogInfo: {
        "id": `fake-list-${i}`,
        "dogName": titles[i % 8],
        "dateOfBirth": dob[i % 8],
        "gender": gender[i % 8],
        "microchipNumber": "1234",
        "breed": "German Shepard",
        "colors": [
          0
        ],
        "ownerId": 0,
        "coOwnerId": 0,
        "sireId": 0,
        "damId": 0,
        "owner": {
          "firstName": 'Test',
          "email": "testing@gmail.com"
        }
      },
      tags: [
        {
          key: '0',
          label: 'Very thoughtful',
        },
        {
          key: '1',
          label: 'Focus on design',
        },
        {
          key: '2',
          label: 'Spicy~',
        },
        {
          key: '3',
          label: 'Big long legs',
        },
        {
          key: '4',
          label: 'Chuanmeizi',
        },
        {
          key: '5',
          label: 'Haina Baichuan',
        },
      ],
      owner: user[i % 10],
      registerBy: registered[i % 5],
      title: titles[i % 8],
      overnightRequested: night[i % 8],
      avatar: avatars[i % 8],
      cover: parseInt(i / 4, 10) % 2 === 0 ? covers[i % 4] : covers[3 - (i % 4)],
      status: ['active', 'exception', 'normal'][i % 3],
      percent: Math.ceil(Math.random() * 50) + 50,
      logo: avatars[i % 8],
      href: 'https://ant.design',
      registrationStatus: statuss[i % 8],
      updatedAt: new Date(new Date().getTime() - 1000 * 60 * 60 * 2 * i),
      createdAt: new Date(new Date().getTime() - 1000 * 60 * 60 * 2 * i),
      subDescription: desc[i % 5],
      description:
        '在中台产品的研发过程中，会出现不同的设计规范和实现方式，但其中往往存在很多类似的页面和组件，这些类似的组件会被抽离成一套标准规范。',
      activeUser: Math.ceil(Math.random() * 100000) + 100000,
      newUser: Math.ceil(Math.random() * 1000) + 1000,
      star: Math.ceil(Math.random() * 100) + 100,
      like: Math.ceil(Math.random() * 100) + 100,
      message: Math.ceil(Math.random() * 10) + 10,
      submittedBy: {
        id: 0,
        oktaId: "DUMMY",
        loginName: "dummyRepresentativeUser1@abkconline.com",
        roles: [
          {
            name: registered[i % 5],
            roleTypeId: 0
          }
        ]
      },
      content:
        '段落示意：蚂蚁金服设计平台 ant.design，用最小的工作量，无缝接入蚂蚁金服生态，提供跨越设计与开发的体验解决方案。蚂蚁金服设计平台 ant.design，用最小的工作量，无缝接入蚂蚁金服生态，提供跨越设计与开发的体验解决方案。',
      members: [
        {
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/ZiESqWwCXBRQoaPONSJe.png',
          name: '曲丽丽',
          id: 'member1',
        },
        {
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/tBOxZPlITHqwlGjsJWaF.png',
          name: '王昭君',
          id: 'member2',
        },
        {
          avatar: 'https://gw.alipayobjects.com/zos/rmsportal/sBxjgqiuHMGRkIjqlQCd.png',
          name: '董娜娜',
          id: 'member3',
        },
      ],
    });
  }

  return list;
}

let sourceData;

function getFakeList(req, res) {
  const params = req.query;

  const count = params.count * 1 || 20;

  const result = fakeList(count);
  sourceData = result;
  return res.json(result);
}

function postFakeList(req, res) {
  const { /* url = '', */ body } = req;
  // const params = getUrlParams(url);
  const { method, id } = body;
  // const count = (params.count * 1) || 20;
  let result = sourceData;

  switch (method) {
    case 'delete':
      result = result.filter(item => item.id !== id);
      break;
    case 'update':
      result.forEach((item, i) => {
        if (item.id === id) {
          result[i] = Object.assign(item, body);
        }
      });
      break;
    case 'post':
      result.unshift({
        body,
        id: `fake-list-${result.length}`,
        createdAt: new Date().getTime(),
      });
      break;
    default:
      break;
  }

  return res.json(result);
}

const getNotice = [
  {
    id: 'xxx1',
    title: titles[0],
    logo: avatars[0],
    description: 'It’s an inner thing that they can’t reach and can’t reach.',
    updatedAt: new Date(),
    member: 'Scientific moving bricks',
    href: '',
    memberLink: '',
  },
  {
    id: 'xxx2',
    title: titles[1],
    logo: avatars[1],
    description: 'Hope is a good thing, maybe the best, good things will not die.',
    updatedAt: new Date('2017-07-24'),
    member: 'The whole group is Wu Yanzu',
    href: '',
    memberLink: '',
  },
  {
    id: 'xxx3',
    title: titles[2],
    logo: avatars[2],
    description: 'There are so many pubs in the town, but she walked into my pub.',
    updatedAt: new Date(),
    member: 'Secondary 2 girls group',
    href: '',
    memberLink: '',
  },
  {
    id: 'xxx4',
    title: titles[3],
    logo: avatars[3],
    description: 'At that time, I only thought about what I wanted, and I didn’t want to have what I ',
    updatedAt: new Date('2017-07-23'),
    member: 'Scientific moving bricks',
    href: '',
    memberLink: '',
  },
  {
    id: 'xxx5',
    title: titles[4],
    logo: avatars[4],
    description: 'Winter is coming',
    updatedAt: new Date('2017-07-23'),
    member: 'Secondary 2 girls group',
    href: '',
    memberLink: '',
  },
  {
    id: 'xxx6',
    title: titles[5],
    logo: avatars[5],
    description: 'Life is like a box of chocolates, and the results are often unexpected.',
    updatedAt: new Date('2017-07-23'),
    member: 'The whole group is Wu Yanzu',
    href: '',
    memberLink: '',
  },
];

const getActivities = [
  {
    id: 'trend-1',
    updatedAt: new Date(),
    user: {
      name: '曲丽丽',
      avatar: avatars2[0],
    },
    group: {
      name: '高逼格设计天团',
      link: 'http://github.com/',
    },
    project: {
      name: '六月迭代',
      link: 'http://github.com/',
    },
    template: '在 @{group} 新建项目 @{project}',
  },
  {
    id: 'trend-2',
    updatedAt: new Date(),
    user: {
      name: '付小小',
      avatar: avatars2[1],
    },
    group: {
      name: '高逼格设计天团',
      link: 'http://github.com/',
    },
    project: {
      name: '六月迭代',
      link: 'http://github.com/',
    },
    template: '在 @{group} 新建项目 @{project}',
  },
  {
    id: 'trend-3',
    updatedAt: new Date(),
    user: {
      name: '林东东',
      avatar: avatars2[2],
    },
    group: {
      name: '中二少女团',
      link: 'http://github.com/',
    },
    project: {
      name: '六月迭代',
      link: 'http://github.com/',
    },
    template: '在 @{group} 新建项目 @{project}',
  },
  {
    id: 'trend-4',
    updatedAt: new Date(),
    user: {
      name: '周星星',
      avatar: avatars2[4],
    },
    project: {
      name: '5 月日常迭代',
      link: 'http://github.com/',
    },
    template: '将 @{project} 更新至已发布状态',
  },
  {
    id: 'trend-5',
    updatedAt: new Date(),
    user: {
      name: '朱偏右',
      avatar: avatars2[3],
    },
    project: {
      name: '工程效能',
      link: 'http://github.com/',
    },
    comment: {
      name: '留言',
      link: 'http://github.com/',
    },
    template: '在 @{project} 发布了 @{comment}',
  },
  {
    id: 'trend-6',
    updatedAt: new Date(),
    user: {
      name: '乐哥',
      avatar: avatars2[5],
    },
    group: {
      name: '程序员日常',
      link: 'http://github.com/',
    },
    project: {
      name: '品牌迭代',
      link: 'http://github.com/',
    },
    template: '在 @{group} 新建项目 @{project}',
  },
];

function getFakeCaptcha(req, res) {
  return res.json('captcha-xxx');
}

export default {
  'GET /api/project/notice': getNotice,
  'GET /api/activities': getActivities,
  'POST /api/forms': (req, res) => {
    res.send({ message: 'Ok' });
  },
  'GET /api/tags': mockjs.mock({
    'list|100': [{ name: '@city', 'value|1-100': 150, 'type|0-2': 1 }],
  }),
  'GET /api/fake_list': getFakeList,
  'POST /api/fake_list': postFakeList,
  'GET /api/captcha': getFakeCaptcha,
};
