import React from 'react'
import ReactDOM from 'react-dom'
import 'bulma/css/bulma.css'
import { Routes } from './Routes/Routes'

class App extends React.Component {
    render() {
        return (
        	<Routes />
        )
    }
}

ReactDOM.render(
	<App/>, 
	document.getElementById('app')
)
module.hot.accept();