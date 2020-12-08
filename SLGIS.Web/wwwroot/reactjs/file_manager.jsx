'use strict';

class NameForm extends React.Component {
    constructor(props) {
        super(props);
        this.state = { value: this.props.name };

        this.handleChange = this.handleChange.bind(this);
    }

    handleChange(event) {
        this.setState({ value: event.target.value });
    }

    save() {
        this.props.onSave(this.state.value);
    }

    render() {
        return (
            <form>
                <input type="text" value={this.state.value}
                    onChange={this.handleChange}
                    onBlur={() => this.save()}
                />
            </form>
        );
    }
}

// file or folder
class Item extends React.Component {
    constructor(props) {
        super(props);
        this.state = { expand: false, title: "" };
    }

    rename(newName) {
        if (this.props.item.title !== newName)
            this.props.onChangeName(newName);
    }

    delete() {
        if (!confirm("Bạn có chắc chắn muốn xóa?"))
            return;

        this.props.onDelete();
    }

    renderFile(item) {
        return (
            <span className="text-secondary pointer">
                <i className={item.isPrivate ? "fa fa-file text-danger  mr-1" : "fa fa-file mr-1"}></i>
                <a href={"/api/fileManager/" + item.id + "/Download"} target="_blank">{item.title}</a>
                <span className="ml-4">
                    <a className="ml-2 text-primary" onClick={() => this.props.toggleShare()}>{item.isPrivate ? "Chia sẻ" : "Hủy chia sẻ"}</a>
                    <a className="ml-2 text-danger" onClick={() => this.delete()}>Xóa</a>
                </span>
            </span>
        )
    }

    renderFolder(item) {
        return (
            <span className="text-secondary pointer">
                <i className={this.state.expand ? "fa fa-minus mr-1" : "fa fa-plus  mr-1"} onClick={() => this.toggleFolder()}> </i>
                <i className="text-warning fa fa-folder mr-1"></i>

                <NameForm name={item.title} onSave={(name) => this.rename(name)} />
                <i className="text-danger fa fa-times" onClick={() => this.delete()}></i>
                {this.state.expand ? <ItemList id={item.id} /> : ""}
            </span>
        )
    }

    toggleFolder() {
        this.setState({ expand: !this.state.expand });
    }

    render() {
        const item = this.props.item;
        if (item.isFolder) {
            return this.renderFolder(item);
        } else {
            return this.renderFile(item);
        }
    }
}

// list files
class ItemList extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            isLoaded: false,
            folders: [],
        };
    }

    componentDidMount() {
        fetch(`/api/fileManager/${this.props.id}`)
            .then(res => res.json())
            .then(
                (result) => {
                    this.setState({
                        isLoaded: true,
                        folders: result
                    });
                },
                (error) => {
                    this.setState({
                        isLoaded: true,
                        error
                    });
                }
            )
    }

    renderItem(item) {
        return (
            <Item item={item}
                onDelete={() => this.deleteFile(item)}
                toggleShare={() => this.toggleShareFile(item.id)}
                onChangeName={(name) => this.rename(item.id, name)}
            />
        );
    }

    rename(id, name) {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: `"${name}"`
        };

        fetch(`/api/fileManager/${id}/rename`, requestOptions)
            .then((_) => {
                const items = this.state.folders.map(item => {
                    if (item.id === id) {
                        item.title = name;
                    }

                    return item;
                });

                this.setState({
                    folders: items
                });
            });
    }

    deleteFile(item) {
        const requestOptions = {
            method: 'DELETE'
        };

        fetch(`/api/fileManager/${item.id}`, requestOptions)
            .then((result) => {
                if (result.status === 400) {
                    alert("Không thể xóa folder đang chứa file hoặc folder!");
                    return;
                }

                const items = this.state.folders.filter(m => m.id !== item.id);
                this.setState({
                    folders: items
                });
            });
    }

    toggleShareFile(id) {
        const requestOptions = {
            method: 'POST'
        };

        fetch(`/api/fileManager/${id}/toggleShare`, requestOptions)
            .then((_) => {
                const items = this.state.folders.map(item => {
                    if (item.id === id) {
                        item.isPrivate = !item.isPrivate;
                    }

                    return item;
                });

                this.setState({
                    folders: items
                });
            });
    }

    handleCreateFolder() {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: `"${this.props.id ? this.props.id : "00000000-0000-0000-0000-000000000000"}"`
        };

        fetch(`/api/fileManager/createFolder`, requestOptions)
            .then(res => res.json())
            .then((result) => {
                const items = this.state.folders;
                items.push(result);

                this.setState({
                    folders: items
                });
            });
    }

    handleUploadFile(files) {
        const formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
        }

        if (this.props.id) {
            formData.append('parentId', this.props.id)
        }

        const requestOptions = {
            headers: { 'content-type': 'application/x-www-form-urlencoded' },
        };

        axios.post('/api/fileManager/upload', formData, requestOptions)
            .then((result) => {
                const items = this.state.folders.concat(result.data);

                this.setState({
                    folders: items
                });
            });
    }

    render() {
        const { error, isLoaded, folders } = this.state;
        if (error) {
            return <div>Error: {error.message}</div>;
        } else if (!isLoaded) {
            return <div>Loading...</div>;
        } else {
            return (
                <React.Fragment>
                    <FolderAction
                        onCreateFolder={() => this.handleCreateFolder()}
                        onUploadFile={(files) => this.handleUploadFile(files)}
                    />
                    <ul>
                        {folders.map(item => (
                            <li key={item.id} className="pb-1">
                                {this.renderItem(item)}
                            </li>
                        ))}
                    </ul>
                </React.Fragment>
            );
        }
    }
}

class FolderAction extends React.Component {
    constructor(props) {
        super(props);
        this.inputFileRef = React.createRef();
    }

    onFileChange(e) {
        e.preventDefault()
        this.props.onUploadFile(e.target.files)
    }

    uploadFileClick() {
        this.inputFileRef.current.click();
    }

    render() {
        return (
            <p>
                <a href="#" onClick={this.props.onCreateFolder} className="mr-3 text-small">
                    <small><i className="fa fa-plus-circle mr-1"></i>Tạo Folder</small>
                </a>
                <a href="#" onClick={() => this.uploadFileClick()}>
                    <input ref={this.inputFileRef} style={{ display: 'none' }} type='file' multiple onChange={(e) => this.onFileChange(e)} />
                    <small>
                        <i className="fa fa-upload mr-1"></i>
                        Upload
                    </small>
                </a>
            </p>
        )
    }
}



class FileManager extends React.Component {
    showCreateFolderForm() {
        console.log("showCreateFolderForm");
    }

    render() {
        return (
            <React.Fragment>
                <ItemList id="00000000-0000-0000-0000-000000000000" />
            </React.Fragment>
        );
    }
}

let domContainer = document.querySelector('#file-manager');
ReactDOM.render(<FileManager />, domContainer);