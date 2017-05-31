const webpackMerge = require('webpack-merge'),
    commonConfig = require('./webpack.common.js');

module.exports = webpackMerge(commonConfig,
{
    devtool: 'cheap-module-eval-source-map',
    entry: {
        app: './src/main.ts',
        vendor: './src/vendor.ts',
        polyfill: './src/polyfill.ts'
    },
    module: {
        rules: [
            { test: /\.tsx?$/, loaders: [ 'awesome-typescript-loader', 'angular2-template-loader' ] }
        ]
    }
});