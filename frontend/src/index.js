import React from 'react'
import ReactDOM from 'react-dom'
import { BrowserRouter as Router } from 'react-router-dom'
import App from './App'
import GlobalState from './context/GlobalState'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'react-image-gallery/styles/css/image-gallery.css'

/**
 * Starting point
 */
ReactDOM.render(
  <React.StrictMode>
    <Router>
      <GlobalState>
        <App />
      </GlobalState>
    </Router>
  </React.StrictMode>
  ,
  document.getElementById('root')
)
