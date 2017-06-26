const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
//const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const bundleOutputDir = './wwwroot/dist';

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [{
        stats: { modules: false },
        entry: { 'main': './ClientApp/main.js' },
        resolve: {
            extensions: ['.js', '.ts', '.vue'],
            alias: {
                'src': path.resolve(__dirname, '../ClientApp'),
                'assets': path.resolve(__dirname, '../ClientApp/assets'),
                'components': path.resolve(__dirname, '../ClientApp/components'),
                'vue': 'vue/dist/vue.common.js',
                'vue$': 'vue/dist/vue.common.js'
            }
        },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            publicPath: '/dist/'
        },
        module: {
            rules: [
                //{ test: /\.ts$/, include: /ClientApp/, use: 'awesome-typescript-loader?silent=true' },
                { test: /\.html$/, use: 'raw-loader' },
                { test: /\.css$/, use:  ['style-loader', 'css-loader'] },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' },
                { test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/, use: 'url-loader?limit=25000' },
                { test: /\.vue$/, use: 'vue-loader' },
            ]
        },
        plugins: [
            /*
            new CheckerPlugin(),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            })*/
            new webpack.ProvidePlugin({ $: 'jquery', jQuery: 'jquery' })
        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(bundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
            // Plugins that apply in production builds only
            new webpack.optimize.UglifyJsPlugin({
                compressor: {
                    warnings: false
                }
            }),
            new ExtractTextPlugin('site.css'),
            new webpack.DefinePlugin({
                'process.env': {
                    'NODE_ENV': JSON.stringify('production')
                }
            })
        ])
    }];
};
