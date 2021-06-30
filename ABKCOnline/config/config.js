// https://umijs.org/config/
import os from 'os';
import pageRoutes from './router.config';
import webpackPlugin from './plugin.config';
import defaultSettings from '../src/defaultSettings';
import slash from 'slash2';

const plugins = [
  [
    'umi-plugin-react',
    {
      antd: true,
      dva: {
        hmr: true,
      },
      locale: {
        enable: true, // default false
        default: 'en-US', // default en-US
        baseNavigator: true, // default true, when it is true, will use `navigator.language` overwrite default
      },
      dynamicImport: {
        loadingComponent: './components/PageLoading/index',
        webpackChunkName: true,
      },
      pwa: {
        workboxPluginMode: 'InjectManifest',
        workboxOptions: {
          importWorkboxFrom: 'local',
        },
        manifestOptions: {
          srcPath: 'src/manifest.json'
        }
      },
      ...(!process.env.TEST && os.platform() === 'darwin' ? {
        dll: {
          include: ['dva', 'dva/router', 'dva/saga', 'dva/fetch'],
          exclude: ['@babel/runtime'],
        },
        hardSource: false,
      } : {}),
    },
  ],
];

//switched to use ABKCOnline google analytics ID
if (process.env.APP_TYPE === 'site') {
  plugins.push([
    'umi-plugin-ga',
    {
      code: 'UA-138643449-1',
    },
  ]);
}

export default {
  // add for transfer to umi
  plugins,
  define: {
    APP_TYPE: process.env.APP_TYPE || '',
  },
  treeShaking: true,
  targets: {
    ie: 11,
  },
  // 路由配置
  routes: pageRoutes,
  // Theme for antd
  // https://ant.design/docs/react/customize-theme-cn
  theme: {
    'primary-color': defaultSettings.primaryColor,
  },
  externals: {
    '@antv/data-set': 'DataSet',
  },
  proxy: {
    '/api': {
      target: 'https://api.abkconline.com/',
      changeOrigin: true,
      secure: true,
      // cookieDomainRewrite: "localhost",
      onProxyReq: proxyReq => {
        // Browsers may send Origin headers even with same-origin
        // requests. To prevent CORS issues, we have to change
        // the Origin to match the target URL.
        if (proxyReq.getHeader('origin')) {
          proxyReq.setHeader('origin', 'https://api.abkconline.com/');
        }
      }
    },
    // '/representatives': {
    //   target: 'https://api.abkconline.com/',
    //   changeOrigin: true,
    // },
    // '/owners': {
    //   target: 'https://api.abkconline.com/',
    //   changeOrigin: true,
    // },
    // '/user': {
    //   target: 'http://test.abkconline.com/',
    //   changeOrigin: false,
    // },
  },
  ignoreMomentLocale: true,
  lessLoaderOptions: {
    javascriptEnabled: true,
  },
  disableRedirectHoist: true,
  cssLoaderOptions: {
    modules: true,
    getLocalIdent: (context, localIdentName, localName) => {
      if (
        context.resourcePath.includes('node_modules') ||
        context.resourcePath.includes('ant.design.pro.less') ||
        context.resourcePath.includes('global.less')
      ) {
        return localName;
      }
      const match = context.resourcePath.match(/src(.*)/);
      if (match && match[1]) {
        const antdProPath = match[1].replace('.less', '');
        const arr = slash(antdProPath)
          .split('/')
          .map(a => a.replace(/([A-Z])/g, '-$1'))
          .map(a => a.toLowerCase());
        return `antd-pro${arr.join('-')}-${localName}`.replace(/--/g, '-');
      }
      return localName;
    },
  },
  manifest: {
    basePath: '/',
  },
  //minimizer
  uglifyJSOptions: {
    sourceMap: true,
  },

  chainWebpack: webpackPlugin,
};
