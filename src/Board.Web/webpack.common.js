const webpack = require('webpack'),
    path = require('path'),
    HtmlWebpackPlugin = require('html-webpack-plugin'),
    CleanWebpackPlugin = require('clean-webpack-plugin');

module.exports = {
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: '[name].[chunkhash].js'
    },
    resolve: {
        extensions: ['.js', '.ts', '.scss', '.html', '.css' ]
    },
    module: {
        rules: [
            { test: /\.html$/, loader: 'raw-loader'},
            { test: /\.(scss|sass)$/, loader: 'raw-loader!sass-loader' }
        ]
    },
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            names: ['app', 'vendor', 'polyfill']
        }),
        new HtmlWebpackPlugin({
            template: './src/index.html'
        }),
        new CleanWebpackPlugin([path.resolve(__dirname, 'wwwroot')], { verbose: true }),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery'
        })
    ]
}