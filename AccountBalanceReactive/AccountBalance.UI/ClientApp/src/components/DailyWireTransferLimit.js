import React, { Component } from 'react';

export class DailyWireTransferLimit extends Component {

    state = {
        accountId: '',
        dailyWireTransferLimit: 0.00,
        message: ''
    };

    // componentWillReceiveProps(){
    //     this.setState({overdraftLimit: this.props.account[0].overdraftLimit});
    //     console.log("componentWillReceiveProps");
    // }

    componentWillReceiveProps(){
        this.setState({dailyWireTransferLimit: this.props.account[0].dailyWireTransferLimit});
        //console.log(this.state.overdraftLimit);
    }

    componentDidMount() {
        this.isRefreshing = true;
    }

    setDailyWireTransferLimit = (event) => {
        event.preventDefault();
        this.postData('api/Home/SetDailyWireTransferLimit', {accountId: this.props.account[0].accountId, dailyWireTransferLimit: this.state.dailyWireTransferLimit})
            .then(data => this.setState({message: data})) //JSON-string from `response.json()` call
            .catch(error => console.error(error));
        
            //this.props.forcefulUpdate();
        // this.props.setLoadAccountsFlag(true);
        // this.isRefreshing = true;
    }

    postData = (url = '', data = {}) => {
        // Default options are marked with *
        return fetch(url, {
                method: 'POST', // *GET, POST, PUT, DELETE, etc.
                mode: 'cors', // no-cors, cors, *same-origin
                cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
                credentials: 'same-origin', // include, *same-origin, omit                
                headers: {
                    'Content-Type': 'application/json',//'application/json',
                    // 'Content-Type': 'application/x-www-form-urlencoded',
                },
                redirect: 'follow', // manual, *follow, error
                referrer: 'no-referrer', // no-referrer, *client
                body: JSON.stringify(data), // body data type must match "Content-Type" header
            })
            .then(response => response.json()); //response.json() parses JSON response into native Javascript objects 
    }

    limitOnChange = (event) => {
        // console.log("limitOnChange");
        // console.log(this.isRefreshing);
        // if(this.isRefreshing === true)
        // {
        //     this.props.setLoadAccountsFlag(false);
        //     this.isRefreshing = false;
        // }
        this.setState({dailyWireTransferLimit: event.target.value})
    }

    render(){
        //console.log("overdraft limit rendered");
        return(
            <div>
                <table className='table table-striped'>
                    <tbody>
                        <tr>
                            <td>Daily wire transfer limit:</td>
                            <td><input type="text" value={this.state.dailyWireTransferLimit} onChange={event => this.limitOnChange(event)}></input></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><button onClick={this.setDailyWireTransferLimit}>Set limit</button></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>{this.state.message}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}