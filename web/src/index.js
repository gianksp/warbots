import React from 'react'
import ReactDOM from 'react-dom'
import 'bulma/css/bulma.css'
import { Routes } from './Routes/Routes'

const title = "Warbots"
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