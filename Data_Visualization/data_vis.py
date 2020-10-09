import pandas as pd
import plotly.express as px
import plotly.graph_objects as go

class VisGraphs:
    def __init__(self, user_id, controller_csv, grab_csv, gaze_csv, pos_csv):

        self.user_id = user_id

        self.controller_data = controller_csv
        self.grab_data = grab_csv
        self.gaze_data = gaze_csv
        self.pos_data = pos_csv
        
        self.controllers_point_data = {}
        self.controllers_dicts = {}
        self.grab_dicts = {}
        self.gaze_dict = {}
        
    

        self.analyze_data()


    def update_data(self, user_id, control, grab, gaze, pos):
        self.user_id = user_id
        self.controller_data = control
        self.grab_data = grab
        self.gaze_data = gaze
        self.pos_data = pos
        self.analyze_data()

    def analyze_controller(self, controller_name):
        
        if not self.user_id: return

        objects = self.controller_data["ObjectName"]
        inter_init = self.controller_data["StartTime"]
        inter_end = self.controller_data["EndTime"]

        self.controllers_point_data[controller_name] = self.controller_data[controller_name]
        self.controllers_dicts[controller_name] = {'total_length_of_time': {}, 
            'total_count': {}, 'avg_look_time': {}, 'max_look_time': {},
            'all_look_times': {}}
            
        for i in range(0, len(self.controllers_point_data[controller_name])):
            if self.controllers_point_data[controller_name][i] == 1:
                obj = objects[i]
                obj_interaction_time = inter_end[i] - inter_init[i]
                
                # Fill out the all_look_times with all look times
                if obj in self.controllers_dicts[controller_name]['all_look_times'].keys():
                    self.controllers_dicts[controller_name]['all_look_times'][obj] \
                        .append(obj_interaction_time)
                else:
                    all_look_times_list = [obj_interaction_time]
                    self.controllers_dicts[controller_name]['all_look_times'][obj] = all_look_times_list
                
                # Fill out the total_length_of_time dictionary
                if obj in self.controllers_dicts[controller_name]['total_length_of_time'].keys():
                    self.controllers_dicts[controller_name]['total_length_of_time'] \
                        [obj] += obj_interaction_time
                else:
                    self.controllers_dicts[controller_name]['total_length_of_time'] \
                        [obj] = obj_interaction_time
                    
                # Fill out the total_count dictionary
                if obj in self.controllers_dicts[controller_name]['total_count'].keys():
                    self.controllers_dicts[controller_name]['total_count'][obj] += 1
                else:
                    self.controllers_dicts[controller_name]['total_count'][obj] = 1
                    
                # Maybe update the longest_look_time
                if obj in self.controllers_dicts[controller_name]['max_look_time'].keys():
                    if self.controllers_dicts[controller_name]['max_look_time'] \
                        [obj] < obj_interaction_time:
                        self.controllers_dicts[controller_name]['max_look_time'] \
                            [obj] = obj_interaction_time
                else:
                    self.controllers_dicts[controller_name]['max_look_time'] \
                        [obj] = obj_interaction_time
                    
        for obj_key in self.controllers_dicts[controller_name]['total_length_of_time'].keys():
            self.controllers_dicts[controller_name]['avg_look_time'][obj_key] = self.controllers_dicts[controller_name]['total_length_of_time'] \
                [obj_key] / self.controllers_dicts[controller_name]['total_count'][obj_key]

    def analyze_grab(self, controller_name):

        if not self.user_id: return

        self.grab_data['InterTime'] = self.grab_data['EndTime'] - self.grab_data['StartTime']
        grab_df = self.grab_data.loc[self.grab_data[controller_name] == 1]

        self.grab_dicts[controller_name] = {'total_length_of_time': {}, 
            'total_count': {}, 'avg_look_time': {}, 'max_look_time': {},
            'all_look_times': {}}
        objs = set(grab_df['ObjectName'])
        
        for obj in objs:
            obj_df = grab_df.loc[grab_df['ObjectName'] == obj]
            times_list = obj_df['InterTime'].tolist()
            self.grab_dicts[controller_name]['all_look_times'][obj] = times_list
            total_time = sum(times_list)
            self.grab_dicts[controller_name]['total_length_of_time'][obj] = total_time
            total_count = len(times_list)
            self.grab_dicts[controller_name]['total_count'][obj] = total_count
            self.grab_dicts[controller_name]['max_look_time'][obj] = max(times_list)
            self.grab_dicts[controller_name]['avg_look_time'][obj] = total_time / total_count

    def analyze_gaze(self):

        if not self.user_id: return

        self.gaze_data['InterTime'] = self.gaze_data['EndTime'] - self.gaze_data['StartTime']
        gaze_df = self.gaze_data

        self.gaze_dict = {'total_length_of_time': {}, 
            'total_count': {}, 'avg_look_time': {}, 'max_look_time': {},
            'all_look_times': {}}
        objs = set(gaze_df['ObjectName'])
        
        for obj in objs:
            obj_df = gaze_df.loc[gaze_df['ObjectName'] == obj]
            times_list = obj_df['InterTime'].tolist()
            self.gaze_dict['all_look_times'][obj] = times_list
            total_time = sum(times_list)
            self.gaze_dict['total_length_of_time'][obj] = total_time
            total_count = len(times_list)
            self.gaze_dict['total_count'][obj] = total_count
            self.gaze_dict['max_look_time'][obj] = max(times_list)
            self.gaze_dict['avg_look_time'][obj] = total_time / total_count

    def analyze_data(self):

        if not self.user_id: return

        self.analyze_controller("LeftControllerPoint")
        self.analyze_controller("RightControllerPoint")

        self.analyze_grab("LeftControllerGrab")
        self.analyze_grab("RightControllerGrab")

        self.analyze_gaze()
    
    def plot_controller_point_avg(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}
        label = list(self.controllers_dicts[controller_name]['total_count'].keys())
        data = list(self.controllers_dicts[controller_name]['avg_look_time'].values())

        df = pd.DataFrame(data={'Objects': label, 'Point Time': data})

        title = ('Left' if controller_name == 'LeftControllerPoint' else 'Right') \
            + ' Controller: Average Point Time'
        fig = px.bar(df, x='Objects', y='Point Time', title=title)
        return fig

    def plot_controller_point_total(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}

        label = list(self.controllers_dicts[controller_name]['total_count'].keys())
        time_data = list(self.controllers_dicts[controller_name]['total_length_of_time'].values())
        count_data = list(self.controllers_dicts[controller_name]['total_count'].values())

        title = ('Left' if controller_name == 'LeftControllerPoint' else 'Right') \
            + ' Controller: Total Point Time and Count'
        
        time_fig = go.Bar(x=label, y=time_data, name="Total Point Time")
        count_fig = go.Bar(x=label, y=count_data, name="Total Point Count")

        fig = go.Figure(data=[time_fig, count_fig])
        fig.update_xaxes(title_text='Objects')
        fig.layout.title.text = title
        return fig

    def plot_box_controller_point(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}


        df = pd.DataFrame(columns=['Objects', 'Point Time'])
        for _, (obj, time_arr) in enumerate(self.controllers_dicts[controller_name]['all_look_times'].items()):
            for time in time_arr:
                df = df.append({'Objects': obj, 'Point Time': time}, ignore_index=True)

        fig = px.box(df, x='Objects', y='Point Time')

        name = "Left Controller" if controller_name == 'LeftControllerPoint' else "Right Controller"
        fig.layout.title.text = "Box Plot: Duration of " + name + " Points at Object"
        return fig

    def plot_both_controller_point(self):

        if not self.user_id: return {'data': [], 'layout': {}}, {'data': [], 'layout': {}}, {'data': [], 'layout': {}}


        avg_time = list(self.controllers_dicts['LeftControllerPoint']['avg_look_time'].values())
        avg_time.extend(list(self.controllers_dicts['RightControllerPoint']['avg_look_time'].values()))

        total_time = list(self.controllers_dicts['LeftControllerPoint']['total_length_of_time'].values())
        total_time.extend(list(self.controllers_dicts['RightControllerPoint']['total_length_of_time'].values()))

        total_count = list(self.controllers_dicts['LeftControllerPoint']['total_count'].values())
        total_count.extend(list(self.controllers_dicts['RightControllerPoint']['total_count'].values()))

        indices = list(self.controllers_dicts['LeftControllerPoint']['total_length_of_time'].keys())
        indices.extend(list(self.controllers_dicts['RightControllerPoint']['total_length_of_time'].keys()))

        avg_fig = go.Figure(go.Bar(x=indices, y=avg_time))
        avg_fig.layout.title.text = 'Both Controllers: Average Point Time'
        avg_fig.update_xaxes(title_text='Objects')

        total_title = 'Both Controller Point: Total Point Time and Count'

        total_time_fig = go.Bar(x=indices, y=total_time, name='Total Time')
        total_count_fig = go.Bar(x=indices, y=total_count, name='Total Count')
        total_fig = go.Figure(data=[total_time_fig, total_count_fig])
        total_fig.update_xaxes(title_text='Objects')
        total_fig.layout.title.text = total_title

        df = pd.DataFrame(columns=['Objects', 'Point Time'])
        for controller_name in ['LeftControllerPoint', 'RightControllerPoint']:
            for _, (obj, time_arr) in enumerate(self.controllers_dicts[controller_name]['all_look_times'].items()):
                for time in time_arr:
                    df = df.append({'Objects': obj, 'Point Time': time}, ignore_index=True)

        box_fig = px.box(df, x='Objects', y='Point Time')
        name = 'Box Plot: Duration of Both Controller Points at Objects'
        box_fig.layout.title.text = name

        return avg_fig, total_fig, box_fig

    def plot_controller_grab_avg(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}

        label = list(self.grab_dicts[controller_name]['total_count'].keys())
        data = list(self.grab_dicts[controller_name]['avg_look_time'].values())

        df = pd.DataFrame(data={'Objects': label, 'Grab Time': data})

        title = ('Left' if controller_name == 'LeftControllerGrab' else 'Right') \
            + ' Controller: Average Grab Time'
        fig = px.bar(df, x='Objects', y='Grab Time', title=title)
        return fig

    def plot_controller_grab_total(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}


        label = list(self.grab_dicts[controller_name]['total_count'].keys())
        time_data = list(self.grab_dicts[controller_name]['total_length_of_time'].values())
        count_data = list(self.grab_dicts[controller_name]['total_count'].values())

        title = ('Left' if controller_name == 'LeftControllerGrab' else 'Right') \
            + ' Controller: Total Grab Time and Count'
        
        time_fig = go.Bar(x=label, y=time_data, name="Total Grab Time")
        count_fig = go.Bar(x=label, y=count_data, name="Total Grab Count")

        fig = go.Figure(data=[time_fig, count_fig])
        fig.update_xaxes(title_text='Objects')
        fig.layout.title.text = title
        return fig

    def plot_box_controller_grab(self, controller_name):

        if not self.user_id: return {'data': [], 'layout': {}}


        df = pd.DataFrame(columns=['Objects', 'Point Time'])
        for _, (obj, time_arr) in enumerate(self.grab_dicts[controller_name]['all_look_times'].items()):
            for time in time_arr:
                df = df.append({'Objects': obj, 'Grab Time': time}, ignore_index=True)

        fig = px.box(df, x='Objects', y='Grab Time')

        name = "Left Controller" if controller_name == 'LeftControllerGrab' else "Right Controller"
        fig.layout.title.text = "Box Plot: Duration of " + name + " Grab at Object"
        return fig

    def plot_both_controller_grab(self):

        if not self.user_id: return {'data': [], 'layout': {}}, {'data': [], 'layout': {}}, {'data': [], 'layout': {}}

        avg_time = list(self.grab_dicts['LeftControllerGrab']['avg_look_time'].values())
        avg_time.extend(list(self.grab_dicts['RightControllerGrab']['avg_look_time'].values()))

        total_time = list(self.grab_dicts['LeftControllerGrab']['total_length_of_time'].values())
        total_time.extend(list(self.grab_dicts['RightControllerGrab']['total_length_of_time'].values()))

        total_count = list(self.grab_dicts['LeftControllerGrab']['total_count'].values())
        total_count.extend(list(self.grab_dicts['RightControllerGrab']['total_count'].values()))

        indices = list(self.grab_dicts['LeftControllerGrab']['total_length_of_time'].keys())
        indices.extend(list(self.grab_dicts['RightControllerGrab']['total_length_of_time'].keys()))

        avg_fig = go.Figure(go.Bar(x=indices, y=avg_time))
        avg_fig.layout.title.text = 'Both Controllers: Average Grab Time'
        avg_fig.update_xaxes(title_text='Objects')

        total_title = 'Both Controller Grab: Total Grab Time and Count'

        total_time_fig = go.Bar(x=indices, y=total_time, name='Total Time')
        total_count_fig = go.Bar(x=indices, y=total_count, name='Total Count')
        total_fig = go.Figure(data=[total_time_fig, total_count_fig])
        total_fig.update_xaxes(title_text='Objects')
        total_fig.layout.title.text = total_title

        df = pd.DataFrame(columns=['Objects', 'Point Time'])
        for controller_name in ['LeftControllerGrab', 'RightControllerGrab']:
            for _, (obj, time_arr) in enumerate(self.grab_dicts[controller_name]['all_look_times'].items()):
                for time in time_arr:
                    df = df.append({'Objects': obj, 'Grab Time': time}, ignore_index=True)

        box_fig = px.box(df, x='Objects', y='Grab Time')
        name = 'Box Plot: Duration of Both Controller Grab at Objects'
        box_fig.layout.title.text = name

        return avg_fig, total_fig, box_fig

    def plot_gaze_avg(self):

        if not self.user_id: return {'data': [], 'layout': {}}

        label = list(self.gaze_dict['total_count'].keys())
        data = list(self.gaze_dict['avg_look_time'].values())

        df = pd.DataFrame(data={'Objects': label, 'Gaze Time': data})

        title = 'Average Gaze Time'
        fig = px.bar(df, x='Objects', y='Gaze Time', title=title)
        return fig        

    def plot_gaze_total(self):

        if not self.user_id: return {'data': [], 'layout': {}}

        label = list(self.gaze_dict['total_count'].keys())
        time_data = list(self.gaze_dict['total_length_of_time'].values())
        count_data = list(self.gaze_dict['total_count'].values())

        title ='Total Gaze Time and Count'
        
        time_fig = go.Bar(x=label, y=time_data, name="Total Gaze Time")
        count_fig = go.Bar(x=label, y=count_data, name="Total Gaze Count")

        fig = go.Figure(data=[time_fig, count_fig])
        fig.update_xaxes(title_text='Objects')
        fig.layout.title.text = title
        return fig

    def plot_box_gaze(self):

        if not self.user_id: return {'data': [], 'layout': {}}


        df = pd.DataFrame(columns=['Objects', 'Point Time'])
        for _, (obj, time_arr) in enumerate(self.gaze_dict['all_look_times'].items()):
            for time in time_arr:
                df = df.append({'Objects': obj, 'Gaze Time': time}, ignore_index=True)

        fig = px.box(df, x='Objects', y='Gaze Time')

        fig.layout.title.text = "Box Plot: Duration of Gaze at Object"
        return fig

    # def plot_pos_heatmap(self):

# if __name__ == '__main__':
#     controller_csv = 'test123_ControllerPointData.csv'
#     grab_csv = 'test123_ControllerGrabData.csv'
#     gaze_csv = 'test123_GazeData.csv'
#     vg = VisGraphs(controller_csv, grab_csv, gaze_csv)
#     vg.plot_controller_grab_avg("LeftControllerGrab")

