import webpack from 'webpack';
import path from 'path';
import WebpackMd5Hash from 'webpack-md5-hash';
import TerserPlugin from 'terser-webpack-plugin';
import MiniCssExtractPlugin from 'mini-css-extract-plugin';
import minimist from 'minimist';
import { styles } from '@ckeditor/ckeditor5-dev-utils';

const mode = minimist(process.argv.slice(2)).hasOwnProperty('production') ? 'production' : 'development';

export default {
  mode: mode,
  resolve: {
    extensions: ['*', '.js', '.json'],
    alias: {
      atauitoolkit: path.resolve(__dirname, 'UI/AtaUiToolkit'),
      ClientsList: path.resolve(__dirname, 'UI/Ata.Investment.ClientsList'),
      KycViewer: path.resolve(__dirname, 'UI/KycViewer')
    }
  },
//  externalsType: 'module',
  externals: {
    'animation': 'global@js/animation.js'
  },
  devtool: mode === 'production' ? 'source-map' : 'inline-source-map',
  entry: {
    _Host: path.resolve(__dirname, 'Ata.Investment.Api/Pages/_Host.js')
  },
  target: 'web',
  output: {
    path: path.resolve(__dirname, 'Ata.Investment.Api/wwwroot/dist'),
    publicPath: '/dist/',
    filename: '[name]-bundle'+(mode === 'production'?'.min':'')+'.js'
  },
  plugins: [
    new MiniCssExtractPlugin({
      // Options similar to the same options in webpackOptions.output
      // both options are optional
      filename: '[name]-bundle'+(mode === 'production'?'.min':'')+'.css'
      // chunkFilename: "[id]-bundle.css"
    })
  ],

  // replacement of CommonChunks plugin
  optimization: {
    runtimeChunk: 'single',
    splitChunks: {
      cacheGroups: {
        vendors: {
          test: /[\\/]node_modules[\\/]/,
          name: 'vendors',
          chunks: 'all'
        }
      }
    },
    minimizer: mode === 'production' ? [
      new TerserPlugin({ // UglifyJS replacement for ES6
        // sourceMap: true,
        terserOptions: {
          ecma: 6,
          output: {
            comments: false
          },
          compress: {
            ecma: 6,
            dead_code: true,
            drop_console:false
          },
        },
        exclude: 'UiUtils.js'
      })
    ] : [],
  },
  module: {
    rules: [
      {test: /\.js$/, exclude: /node_modules/, loader: 'babel-loader'},
      {test: path.resolve(__dirname, 'UI/KycViewer/Shared/UiUtils'), loader: "expose-loader", options: {
          exposes: "uiUtils",
      }},
      {
//          test: /ckeditor5-[^/]+\/theme\/[\w-/]+\.css$/,
        test: /\.css$/,
        use: [
          {
            loader: 'style-loader',
            options: {
              injectType: 'singletonStyleTag'
            }
          },
          {
            loader: 'css-loader'
          },
          {
            loader: 'postcss-loader',
            options: {
              postcssOptions: styles.getPostCssConfig({
                themeImporter: {
                  themePath: require.resolve('@ckeditor/ckeditor5-theme-lark')
                },
                minify: true
              })
            }
          }
        ]
      },
      {
        test: /\.scss$/, exclude: /\.cmp.scss$/, use: [
          MiniCssExtractPlugin.loader,
          // {
          //   loader: 'constructable-style-loader',
          //   options: {
          //     purge: true,
          //     content: [],
          //   }
          // },
          {
            loader: 'css-loader',
            options: {
              sourceMap: true
            }
          },
          // 'postcss-loader',
          {
            loader: 'sass-loader',
            options: {
                sourceMap: true,
                implementation: require('sass')
            }
          }
        ]
      },
      {
        test: /\.cmp.scss$/, use: [
          {
            loader: 'constructable-style-loader',
            options: {
              purge: false,
              content: [],
            }
          },
          // 'css-loader',
          'postcss-loader',
          {
            loader: 'sass-loader',
            options: {
                implementation: require('sass')
            }
          }
        ]
      },
      {test: /\.(html|svg)$/, use: 'raw-loader'},
      {
        test: /\.woff(2)?(\?v=[0-9]\.[0-9]\.[0-9])?$/,
        loader: 'url-loader',
        options: {
          limit: 10000,
          mimetype: 'application/font-woff'
        }
      },
      {
        test: /\.(ttf|otf|eot)(\?v=[0-9]\.[0-9]\.[0-9])?|(jpg|gif|png)$/,
        loader: 'file-loader'
      }
    ]
  }
};
