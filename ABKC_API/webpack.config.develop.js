const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const {
    CheckerPlugin
} = require('awesome-typescript-loader');
const package = require('./package.json');

const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    devtool: 'source-map',
    entry: {
        'site': "./Scripts/App/site.tsx",
        'app': "./Scripts/App/index.tsx",
        'dashboard': './Scripts/App/Dashboard/index.tsx',
        // 'css/site': './Content/scss/src/site.scss'
        // vendor: Object.keys(package.dependencies),
    },
    target: 'web',
    output: {
        publicPath: "./",
        path: path.join(__dirname, 'wwwroot/dist'),
        filename: "js/[name].js"
    },
    plugins: [
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery',
            'window.jQuery': 'jquery',
            Popper: ['popper.js', 'default']
        }),
        new ExtractTextPlugin({
            filename: 'css/[name].css',
            disable: false,
            allChunks: true
        }),
        new webpack.LoaderOptionsPlugin({
            debug: true
        }),
        new CopyWebpackPlugin([{
                from: 'Content/Static',
                to: 'static/',
                force: false
            }

        ], {
            debug: 'info'
        }),
        new CheckerPlugin(),
    ],
    module: {
        rules: [{
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        presets: ['@babel/preset-env', '@babel/preset-react', '@babel/preset-stage-3'],
                    }
                },
            },
            {
                test: /\.css$/,
                use: ExtractTextPlugin.extract({
                    fallback: "style-loader",
                    use: "css-loader"
                })
            },
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract({
                    fallback: 'style-loader',
                    use: ['css-loader?sourceMap', 'sass-loader?sourceMap']
                })
            },
            {
                test: /\.(woff(2)?|ttf|eot)(\?v=[0-9]\.[0-9]\.[0-9])?$/, //to support @font-face rule
                loader: "url-loader",
                query: {
                    limit: '10000',
                    name: '[name].[ext]',
                    outputPath: 'fonts/'
                    //the fonts will be emitted to wwwroot/fonts/ folder
                    //the fonts will be put in the DOM <style> tag as eg. @font-face{ src:url(assets/fonts/font.ttf); }
                }
            },
            {
                test: /\.(jpe?g|png|gif|svg)$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        outputPath: 'images/',
                        name: '[name].[ext]',
                        publicPath: '/wwwroot/images'
                        // publicPath:'../../wwwroot/images/'
                    }
                }]
            },
            {
                enforce: 'pre',
                test: /\.tsx?$/,
                use: "awesome-typescript-loader"
            },
            {
                enforce: 'pre',
                test: /\.tsx?$/,
                loader: "tslint-loader",
                options: {
                    failOnHint: false,
                    formatter: 'stylish'
                }
            }
        ]
    },
    resolve: {
        extensions: [".tsx", ".ts", ".js", ".scss", ".css"]
    }
};