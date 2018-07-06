// var express = require('express')
// var app = express()
// var serveStatic = require('serve-static')
// var path = require('path')
// 
// 
// app.use(function (req, res, next) {
//     var filename = path.basename(req.url);
//     var extension = path.extname(filename);
//     console.log("The file " + filename + " was requested.");
//     next();
// });
// 
// app.use('/static',serveStatic(path.join(__dirname, 'public'))
// //, {
//     // setHeaders: function(res) {
//     //     res.setHeader("Content-Type", "application/json");
//     // }}
// )
// //app.use('/static', express.static(path.join(__dirname, 'public')))
// 
// app.get('/', function (req, res) {
//   res.status(200).send('Hello World!')
// })
// 
// app.get('/data/*', function (req, res) {
//   res.status(200).send('{"Data":"ok"}')
//   console.log('Data:'+req.originalUrl)
// })
// 
// // app.get('/public', function (req, res) {
// //   res.send('Hello World!')
// // })
// 
// app.get('*', function(req, res){
//   console.log('!!!error:'+req.originalUrl)
//   res.status(404).send('404');
// });
// 
// app.listen(8000, function () {
//   console.log('Example app listening on port 8000!')
// })
