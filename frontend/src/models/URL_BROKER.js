// const baseURL = 'https://api-fotoportal.azurewebsites.net/'
const baseURL = 'https://localhost:5001/'

const URL_BROKER = {
  SERVER_BROKER_GET_ORDER_URL: baseURL + 'api/broker',
  SERVER_BROKER_PUT_ORDER_URL: baseURL + 'api/broker/order',
  SERVER_BROKER_GET_PHOTOGRAPHERS_URL: baseURL + 'api/photographer',
  SERVER_BROKER_GET_ONE_PHOTOGRAPHER_URL: baseURL + 'api/broker/photographer',
  SERVER_BROKER_GET_ESTATE_INFO_URL: baseURL + 'api/broker/estateinfo',
  SERVER_BROKER_GET_BROKER_INFO_URL: baseURL + 'api/broker/brokerinfo',
  SERVER_BROKER_GET_DEPARTMENT_INFO: baseURL + 'api/broker/departmentinfo'
}

export default URL_BROKER
