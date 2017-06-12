const path = require('path'),
    webpack = require('webpack'),
    webpackMerge = require('webpack-merge'),
    commonConfig = require('./webpack.common.js'),
    AotPlugin = require('@ngtools/webpack').AotPlugin;

let root = function (args) {
    args = Array.prototype.slice.call(arguments, 0);
    return path.join.apply(path, [__dirname].concat(args));
};

const ENV = process.env.NODE_ENV = process.env.ENV = 'production';

module.exports = webpackMerge(commonConfig,
{
    devtool: 'source-map',
    entry: {
        app: './src/main.ts',
        vendor: './src/vendor-aot.ts',
        polyfill: './src/polyfill.ts'
    },
    module: {
        rules: [
            { test: /\.tsx?$/, loader: '@ngtools/webpack' }
        ]
    },
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            beautify: false,
            comments: false,
            compress: {
                screw_ie8: true,
                warnings: false
            },
            mangle: {
                keep_fnames: true,
                screw_i8: true
            }
        }),
        new webpack.DefinePlugin({
            'process.env': {
                'ENV': JSON.stringify(ENV)
            }
        }),
        new AotPlugin({
            tsConfigPath: root('tsconfig-aot.json'),
            entryModule: root('src', 'app', 'app.module#AppModule'),
            mainPath: root('src', 'main-aot.ts')
        })
    ]
});