const webpack = require('webpack'),
    path = require('path'),
    HtmlWebpackPlugin = require('html-webpack-plugin'),
    CleanWebpackPlugin = require('clean-webpack-plugin'),
    CopyWebpackPlugin = require('copy-webpack-plugin'),
    ExtractTextPlugin = require('extract-text-webpack-plugin'),
    autoprefixer = require('autoprefixer');

let root = function(args) {
  args = Array.prototype.slice.call(arguments, 0);
  return path.join.apply(path, [__dirname].concat(args));
};

module.exports = {
    output: {
        path: path.resolve(__dirname, 'wwwroot'),
        filename: '[name].[chunkhash].js'
    },
    resolve: {
        extensions: ['.js', '.ts', '.scss', '.html', '.css']
    },
    module: {
        rules: [
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.(scss|sass)$/, exclude: root('src', 'style'), loader: 'raw-loader!sass-loader' },
            { test: /\.(scss|sass)$/, exclude: root('src', 'app'),
                loader: ExtractTextPlugin.extract({ 
                    fallback: 'style-loader', 
                    use: [ 
                        { loader: 'css-loader' },
                        { loader: 'postcss-loader', options: {
                            plugins: function () {
                                return [autoprefixer]
                            }
                        }},
                        { loader: 'sass-loader', options: {
                            includePaths: [path.resolve(__dirname, "node_modules/foundation-sites/scss")]
                        }}] 
                    }) 
            },
            { test: /\.(jpeg|jpg|png)(\?.*$|$)/, include: root('src', 'public'), loader: 'file-loader' },
            { test: /\.(ttf|otf|eot|svg|woff(2)?)(\?.*$|$)/, loader: 'file-loader?name=fonts/[name].[ext]' }
        ]
    },
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            names: ['app', 'vendor', 'polyfill']
        }),
        new HtmlWebpackPlugin({
            template: './src/public/index.html'
        }),
        new CleanWebpackPlugin([root('wwwroot')], { verbose: true }),
        new webpack.ProvidePlugin({
            $: 'jquery',
            jQuery: 'jquery'
        }),
        new CopyWebpackPlugin([
            {
                from: './src/public'
            }]),
        new ExtractTextPlugin({
            filename: 'css/[name].[hash].css', 
            disable: process.env.NODE_ENV === "development"
        })
    ]
}