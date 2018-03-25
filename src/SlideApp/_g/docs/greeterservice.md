GreeterService
--------------------------
	问候语句
	
## 1. Service Definition


### 1.1 GreeterService.SayHello 
> 10000.1 
> SayHello 服务  


#### HTTP调用
+ **接口地址** : /api/greeter/sayhello  
+ **接口说明** : SayHello 服务  
+ **请求方式** : GET  



*公共参数不显示，关于公共参数可参考首页说明*

#### 1.1.1 Request


[HelloReq]  Hello的请求消息

|  字段名  |  类型  |  注释  |   JSON Name  |
| ------------ | ------------ | ------------ | ------------ |
|  client_ip  |  string  |  客户端IP  |   clientIp   |
|  identity  |  string  |  用户标识  |   identity   |
|  x_request_id  |  string  |  请求ID  |   xRequestId   |
|  name  |  string  |  名称  |   name   |



#### 1.1.2 Response



[HelloRes]  Hello的请求响应消息

|  字段名  |  类型  |  注释  |   JSON Name  |
| ------------ | ------------ | ------------ | ------------ |
|  return_message  |  string  |  返回消息内容  |   returnMessage   |
|  greet_word  |  string  |  问候语  |   greetWord   |


### 1.2 GreeterService.SayHelloAgain 
> 10000.2 
> SayHelloAgain 服务  


#### HTTP调用
+ **接口地址** : /api/greeter/sayhelloagain  
+ **接口说明** : SayHelloAgain 服务  
+ **请求方式** : GET  



*公共参数不显示，关于公共参数可参考首页说明*

#### 1.2.1 Request


[HelloReq]  Hello的请求消息

|  字段名  |  类型  |  注释  |   JSON Name  |
| ------------ | ------------ | ------------ | ------------ |
|  client_ip  |  string  |  客户端IP  |   clientIp   |
|  identity  |  string  |  用户标识  |   identity   |
|  x_request_id  |  string  |  请求ID  |   xRequestId   |
|  name  |  string  |  名称  |   name   |



#### 1.2.2 Response



[HelloRes]  Hello的请求响应消息

|  字段名  |  类型  |  注释  |   JSON Name  |
| ------------ | ------------ | ------------ | ------------ |
|  return_message  |  string  |  返回消息内容  |   returnMessage   |
|  greet_word  |  string  |  问候语  |   greetWord   |




## 2. Message Definition

### <span id="helloreq">HelloReq</span> 
> Hello的请求消息  

| 字段名     | 类型   |  注释  |  JSON Name  |
| --------   | -----  | ----  | ----  |
|  client_ip  |  string  |  客户端IP  |   clientIp   |
|  identity  |  string  |  用户标识  |   identity   |
|  x_request_id  |  string  |  请求ID  |   xRequestId   |
|  name  |  string  |  名称  |   name   |

### <span id="hellores">HelloRes</span> 
> Hello的请求响应消息  

| 字段名     | 类型   |  注释  |  JSON Name  |
| --------   | -----  | ----  | ----  |
|  return_message  |  string  |  返回消息内容  |   returnMessage   |
|  greet_word  |  string  |  问候语  |   greetWord   |
