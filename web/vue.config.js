module.exports = {
  devServer: {
    proxy: {
      '/api': {
        target: 'http://localhost:5003', 
        changeOrigin: true,
        pathRewrite: { '^/api': '' },
      },
    },
  },
};
