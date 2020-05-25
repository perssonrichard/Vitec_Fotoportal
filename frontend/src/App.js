import React from 'react'
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom'
import PhotographerView from './components/photographer/PhotographerView'
// import Login from './components/photographer/Login'
import Register from './components/photographer/Register'
import BrokerView from './components/broker/BrokerView'

const App = () => {
  return (
    <div className='App'>
      <Router>
        <Switch>
          <Route exact path='/maklare' component={BrokerView} />
          <Route exact path='/admin' component={Register} />
          <Route path='/' component={PhotographerView} />
        </Switch>
      </Router>
    </div>
  )
}

export default App
