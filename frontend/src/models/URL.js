// const baseURL = 'https://api-fotoportal.azurewebsites.net/'
const baseURL = 'http://localhost:5000/'

const URL = {
  SERVER_EDIT_USER_URL: baseURL + 'api/photographer/',
  SERVER_REMOVE_ACCOUNT_URL: baseURL + 'api/photographer/',
  SERVER_EDIT_PASSWORD_URL: baseURL + 'api/photographer/editPassword',
  SERVER_LOGIN_URL: baseURL + 'login',
  SERVER_POST_IMAGE_URL: baseURL + 'api/image',
  SERVER_PUT_IMAGE_URL: baseURL + 'api/image/',
  SERVER_PUT_SORT_IMAGE_URL_1: baseURL + 'api/image/',
  SERVER_PUT_SORT_IMAGE_URL_2: '/sort',
  SERVER_DELETE_IMAGE_URL: baseURL + 'api/image/',
  SERVER_GET_IMAGE_URL: baseURL + 'api/image/',
  SERVER_GET_ORDER_URL: baseURL + 'api/order',
  SERVER_GET_USER_URL: baseURL + 'api/photographer/',
  SERVER_REGISTER_URL: baseURL + 'api/register',
  SERVER_POST_NEXT_URL_1: baseURL + 'api/image/',
  SERVER_POST_NEXT_URL_2: '/send',
  SERVER_POST_BROKER_INFO: baseURL + 'api/photographer/brokerinfo',
  SERVER_POST_DEPARTMENT_INFO: baseURL + 'api/photographer/departmentinfo'
}

export default URL
